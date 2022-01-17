using System;
using System.Collections.Generic;
using System.Data;
using Chloe;
using Chloe.Infrastructure;
using Chloe.MySql;
using Chloe.PostgreSQL;
using Chloe.SqlServer;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MySql.Data.MySqlClient;
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

    private readonly Dictionary<ENUM_DATABASE_TYPE, Func<DatabaseFacade, IDbContext>> _chloeDbState =
        new Dictionary<ENUM_DATABASE_TYPE, Func<DatabaseFacade, IDbContext>>()
        {
            {ENUM_DATABASE_TYPE.MSSQL, (db) => new MsSqlContext(() => db.GetDbConnection()) },
            {ENUM_DATABASE_TYPE.MYSQL, (db) => new MySqlContext(new MySqlConnectionFactory(db.GetDbConnection())) },
            {ENUM_DATABASE_TYPE.POSTGRES, (db) => new PostgreSQLContext(new PostgreSQLConnectionFactory(db.GetConnectionString()) ) },
        };

    private readonly Dictionary<ENUM_DATABASE_TYPE, Func<IDbConnection, QueryFactory>> _sqlKataDbState =
        new Dictionary<ENUM_DATABASE_TYPE, Func<IDbConnection, QueryFactory>>()
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

    public IDbContext UseChloeDbContext()
    {
        var func = this._chloeDbState[DbType];
        if (func.xIsEmpty()) throw new NotImplementedException($"dbtype {this.DbType.ToString()} not implemented.");
        return func(this.Database);
    }

    public QueryFactory UseSqlKata()
    {
        Compiler compiler = null;
        IDbConnection connection = this.Database.GetDbConnection();
        var func = _sqlKataDbState[this.DbType];
        if (func.xIsEmpty()) throw new NotImplementedException($"key {this.DbType.ToString()} not implemented");
        return func(connection);
    }
    
    internal sealed class MySqlConnectionFactory : IDbConnectionFactory
    {
        private string _connString = null;
        private IDbConnection _connection;
        public MySqlConnectionFactory(string connString)
        {
            this._connString = connString;
        }

        public MySqlConnectionFactory(IDbConnection connection)
        {
            this._connection = connection;
        }
        
        public IDbConnection CreateConnection()
        {
            if (this._connection.xIsNotEmpty()) return this._connection;
            
            IDbConnection conn = new MySqlConnection(this._connString);
            return conn;
        }
    }

    internal sealed class PostgreSQLConnectionFactory : IDbConnectionFactory
    {
        private string _connString = null;
        private IDbConnection _connection;
        public PostgreSQLConnectionFactory(string connString)
        {
            this._connString = connString;
        }

        public PostgreSQLConnectionFactory(IDbConnection connection)
        {
            this._connection = connection;
        }
        
        public IDbConnection CreateConnection()
        {
            if (this._connection.xIsNotEmpty()) return this._connection;
            
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
}