using System;
using System.Collections.Generic;
using System.Data;
using Domain.Enums;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql;
using SqlKata.Compilers;
using SqlKata.Execution;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace Application.Context;

/// <summary>
/// MsSql, MySql, Postgres 통합 DbContext
/// </summary>
public class DbContextBase : DbContext
{
    protected readonly IDbConnection DbConnection;
    protected readonly ENUM_DATABASE_TYPE DbType;
    private readonly string _connection;
    private readonly Dictionary<ENUM_DATABASE_TYPE, Func<IDbConnection, QueryFactory>> _sqlKataDbState =
        new()
        {
            { ENUM_DATABASE_TYPE.MSSQL, (con) => new QueryFactory(con, new SqlServerCompiler()) },
            { ENUM_DATABASE_TYPE.MYSQL, (con) => new QueryFactory(con, new MySqlCompiler()) },
            { ENUM_DATABASE_TYPE.POSTGRES, (con) => new QueryFactory(con, new PostgresCompiler()) },
        };
    
    public DbContextBase(DbContextOptions options) : base(options)
    {
        if(this.Database.ProviderName.ToUpper().Contains("SQLSERVER")) this.DbType = ENUM_DATABASE_TYPE.MSSQL;
        //not tested
        else if(this.Database.ProviderName.ToUpper().Contains("MYSQL")) this.DbType = ENUM_DATABASE_TYPE.MYSQL;
        //not tested
        else if(this.Database.ProviderName.ToUpper().Contains("PGSQL")) this.DbType = ENUM_DATABASE_TYPE.POSTGRES;
        else throw new NotImplementedException();
    }
    
    public DbContextBase(string connection)
    {
        _connection = connection;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //base.OnConfiguring(optionsBuilder);
        if (!optionsBuilder.IsConfigured)
        {
            //optionsBuilder.UseSqlServer(_connection);
        }
    }

    public QueryFactory UseSqlKata()
    {
        IDbConnection connection = this.Database.GetDbConnection();
        var func = _sqlKataDbState[this.DbType];
        if (func.xIsEmpty()) throw new NotImplementedException($"key {this.DbType.ToString()} not implemented");
        return func(connection);
    }
}

