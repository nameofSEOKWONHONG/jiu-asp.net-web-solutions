using System.Data.Common;
using Chloe.MySql;
using Chloe.PostgreSQL;
using Chloe.SqlServer;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;
using SqlKata.Compilers;
using SqlKata.Execution;
using SqlSugar;

namespace Application.Context;

public class DbContextBase : DbContext
{
    private readonly DbConnection _dbConnection;
    
    public DbContextBase(DbContextOptions options) : base(options)
    {
        
    }
    
    public DbContextBase(DbConnection connection)
    {
        this._dbConnection = connection;
    }
}