using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Dapper;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;

namespace JUIControls.Services;

/// <summary>
/// sample code, not working
/// </summary>
public class SaleService : ISaleService
{
    private readonly DbContext _context;
    public SaleService(DbContext context)
    {
        _context = context;
    }
    public (string value, Dictionary<string, string> valueOptions) Bind(Dictionary<string, string> options)
    {
        var v = GetSale(int.Parse(options["id"]));
        var vo = GetSaleValueOptions(options["ComCode"]);
        return (v, vo);
    }

    public string GetSale(int id)
    {
        using (var connection = _context.Database.GetDbConnection())
        {
            return connection.xDatabaseTry<string>(connection =>
            {
                var result = connection.QueryFirst<string>("SELECT GETDATE()");
                return (string)result;
            });

            // return connection.xDatabaseTryAsync<string>(async (connection) =>
            // {
            //     var result = await connection.QueryFirstAsync<string>("SELECT GETDATE()");
            //     return result;
            // }).GetAwaiter().GetResult();
        }

        return string.Empty;
    }

    public Dictionary<string, string> GetSaleValueOptions(string comCode)
    {
        return new (null, null);
    }
}