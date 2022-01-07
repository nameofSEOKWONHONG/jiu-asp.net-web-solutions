using System.Data;
using Chloe;
using Chloe.Infrastructure;
using Chloe.MySql;
using Chloe.PostgreSQL;
using Chloe.SqlServer;
using Chloe.SqlServer.Odbc;
using Domain.Entities;
using eXtensionSharp;
using MySql.Data.MySqlClient;
using Npgsql;
using MsSqlContext = Chloe.SqlServer.MsSqlContext;
using MySqlContext = Chloe.MySql.MySqlContext;

namespace Application.Context;

/// <summary>
/// TODO : 리졸버 만들어야 함. 아니면 SQLKATA와 CHLOE를 묶어야 함. SqlSugar는 그대로 사용.
/// <see href="https://github.com/shuxinqin/Chloe"></see>
/// </summary>
public class ChloeDbContext
{
    private readonly DbContext _chloeDbContext;

    public ChloeDbContext(string connection, ENUM_DATABASE_TYPE type)
    {
        if (type == ENUM_DATABASE_TYPE.MSSQL) _chloeDbContext = new MsSqlContext(connection);
        else if (type == ENUM_DATABASE_TYPE.MYSQL) _chloeDbContext = new MySqlContext(new MySqlConnectionFactory(connection));
        else if (type == ENUM_DATABASE_TYPE.POSTGRES)
            _chloeDbContext = new PostgreSQLContext(new PostgreSQLConnectionFactory(connection));
        else throw new NotImplementedException();
    }

    public DbContext GetDbContext()
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