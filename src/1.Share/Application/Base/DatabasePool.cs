using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Application.Base;

public class DatabaseOption
{
    public int MaxPool { get; set; }
    public string ConnectionString { get; set; }
}

public class SqlConnectionBase
{
    public SqlConnection SqlConnection { get; set; }
    public bool IsUse { get; set; }
    public bool IsBase { get; set; } = true;
}

public class DatabasePool : IDisposable
{
    private static object _sync = new object();
    private static object _executeSync = new object();
    private ConcurrentBag<SqlConnectionBase> _sqlConnections;
    private DatabaseOption _databaseOption;
    /// <summary>
    /// (machine memory * 1024 * 1024) / 12582880
    /// </summary>
    private readonly int MAX_POOL_SIZE = (512 * 1024 * 1024) / 12582880;
    public DatabasePool(IOptionsMonitor<DatabaseOption> optionsMonitor)
    {
        _databaseOption = optionsMonitor.CurrentValue;
        optionsMonitor.OnChange(optionsMonitor =>
        {
            _databaseOption = optionsMonitor;
        });
        Initialize();
    }

    private void Initialize()
    {   
        _sqlConnections = new ConcurrentBag<SqlConnectionBase>();//_databaseOption.MaxPool);
        Enumerable.Range(0, MAX_POOL_SIZE/*_databaseOption.MaxPool - 1*/).ToList().ForEach(i =>
        {
            var instance = new SqlConnectionBase()
            {
                SqlConnection = new SqlConnection(_databaseOption.ConnectionString),
                IsUse = false
            };
            instance.SqlConnection.Open();
            instance.SqlConnection.Close();
            _sqlConnections.Add(instance);
        });        
    }

    private SqlConnectionBase GetConnection()
    {
        SqlConnectionBase connectionBase = null;
        
        lock (_sync)
        {
            connectionBase = _sqlConnections.FirstOrDefault(m => m.IsUse == false);
            if (connectionBase != null)
            {
                connectionBase.IsUse = true;
                return connectionBase;
            }
            else
            {
                var instance = new SqlConnectionBase()
                {
                    IsBase = false,
                    IsUse = true,
                    SqlConnection = new SqlConnection(_databaseOption.ConnectionString)
                };
                _sqlConnections.Add(instance);
                return instance;
            }
        }
    }

    public void Execute(Action<SqlConnection> action)
    {
        var conBase = GetConnection();
        {
            conBase.SqlConnection.Open();
            action(conBase.SqlConnection);
            conBase.SqlConnection.Close();
            conBase.IsUse = false;
        }
    }

    public void Reset()
    {
        lock (_sync)
        {
            if (_sqlConnections.Count > this.MAX_POOL_SIZE)
            {
                while (true)
                {
                    _sqlConnections.TryTake(out SqlConnectionBase result);
                    result.IsUse = false;
                    result.SqlConnection.Close();
                    result.SqlConnection.Dispose();
                    if (_sqlConnections.Count <= this.MAX_POOL_SIZE) return;
                }
            }
        }
    }
    
    public void Dispose()
    {
        if (_sqlConnections != null)
        {
            foreach (var sqlConnectionBase in _sqlConnections)
            {
                sqlConnectionBase.IsUse = false;
                sqlConnectionBase.SqlConnection.Close();
                sqlConnectionBase.SqlConnection.Dispose();
            }
            _sqlConnections.Clear();
            _sqlConnections = null;
        }
    }
}