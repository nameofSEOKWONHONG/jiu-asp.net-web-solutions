using System;
using System.Data;
using Chloe;
using Chloe.Infrastructure;
using Chloe.MySql;
using Chloe.PostgreSQL;
using Chloe.SqlServer;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;
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

    private readonly string _connectionString;
    public DbContextBase(DbContextOptions options) : base(options)
    {
        if(this.Database.ProviderName.ToUpper().Contains("SQLSERVER")) this.DbType = ENUM_DATABASE_TYPE.MSSQL;
        //not tested
        else if(this.Database.ProviderName.ToUpper().Contains("MYSQL")) this.DbType = ENUM_DATABASE_TYPE.MYSQL;
        //not tested
        else if(this.Database.ProviderName.ToUpper().Contains("PGSQL")) this.DbType = ENUM_DATABASE_TYPE.POSTGRES;
        else throw new NotImplementedException();
    }
    
    public DbContextBase(IDbConnection connection)
    {
        this.DbConnection = connection;
    }

    public IDbContext UseChloeDbContext()
    {
        if (DbType == ENUM_DATABASE_TYPE.MSSQL) 
            return new MsSqlContext(() => this.Database.GetDbConnection());
        else if (DbType == ENUM_DATABASE_TYPE.MYSQL)
            return new MySqlContext(new MySqlConnectionFactory(this.Database.GetDbConnection()));
        else if (DbType == ENUM_DATABASE_TYPE.POSTGRES)
            return new PostgreSQLContext(new PostgreSQLConnectionFactory(this.Database.GetDbConnection()));
        else throw new NotImplementedException();
    }

    public (IDbConnection connection, QueryFactory queryFactory) UseSqlKata()
    {
        Compiler compiler = null;
        IDbConnection connection = this.Database.GetDbConnection();
        var connectionString = this.Database.GetConnectionString();
        
        if(DbType == ENUM_DATABASE_TYPE.MSSQL) compiler = new SqlServerCompiler();
        else if (DbType == ENUM_DATABASE_TYPE.MYSQL) compiler = new MySqlCompiler();
        else if (DbType == ENUM_DATABASE_TYPE.POSTGRES) compiler = new PostgresCompiler();
        else throw new NotImplementedException();
        
        return new (connection, new QueryFactory(connection, compiler));
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