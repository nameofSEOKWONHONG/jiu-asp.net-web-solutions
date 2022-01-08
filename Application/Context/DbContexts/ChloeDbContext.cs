using System;
using System.Data;
using Chloe;
using Chloe.Infrastructure;
using Chloe.PostgreSQL;
using eXtensionSharp;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Npgsql;
using MsSqlContext = Chloe.SqlServer.MsSqlContext;
using MySqlContext = Chloe.MySql.MySqlContext;

namespace Application.Context;

/// <summary>
/// TODO : 리졸버 만들어야 함. 아니면 SQLKATA와 CHLOE를 묶어야 함. SqlSugar는 그대로 사용.
/// <see href="https://github.com/shuxinqin/Chloe"></see>
/// </summary>
public class ChloeDbContext : CustomDbContextBase
{
    private readonly IDbContext _chloeDbContext;

    public ChloeDbContext(IConfiguration configuration) : base(ENUM_DATABASE_TYPE.MSSQL, configuration)
    {
        if (this.DatabaseType == ENUM_DATABASE_TYPE.MSSQL) _chloeDbContext = new MsSqlContext(this.ConnectionString);
        else if (this.DatabaseType == ENUM_DATABASE_TYPE.MYSQL) _chloeDbContext = new MySqlContext(new MySqlConnectionFactory(this.ConnectionString));
        else if (this.DatabaseType == ENUM_DATABASE_TYPE.POSTGRES)
            _chloeDbContext = new PostgreSQLContext(new PostgreSQLConnectionFactory(this.ConnectionString));
        else throw new NotImplementedException();
    }

    public IDbContext GetDbContext()
    {
        return _chloeDbContext;
    }
}

public class MySqlConnectionFactory : IDbConnectionFactory
{
    string _connString = null;
    public MySqlConnectionFactory(string connString)
    {
        this._connString = connString;
    }
    public IDbConnection CreateConnection()
    {
        IDbConnection conn = new MySqlConnection(this._connString);
        return conn;

    }
}

public class PostgreSQLConnectionFactory : IDbConnectionFactory
{
    string _connString = null;
    public PostgreSQLConnectionFactory(string connString)
    {
        this._connString = connString;
    }
    public IDbConnection CreateConnection()
    {
        NpgsqlConnection conn = new NpgsqlConnection(this._connString);
        return conn;
    }
}

public class ENUM_DATABASE_TYPE : XEnumBase<ENUM_DATABASE_TYPE>
{
    public static readonly ENUM_DATABASE_TYPE MSSQL = Define("MSSQL");
    public static readonly ENUM_DATABASE_TYPE MYSQL = Define("MYSQL");
    public static readonly ENUM_DATABASE_TYPE POSTGRES = Define("POSTGRES");
}