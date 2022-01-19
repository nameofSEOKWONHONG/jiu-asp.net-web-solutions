using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Application.Interfaces.Todo;
using Dapper;
using eXtensionSharp;
using Microsoft.EntityFrameworkCore;

namespace JUIControls.Binders;

/// <summary>
/// sample code, not working
/// </summary>
public class SaleWidgetBinder : ISaleWidgetBinder
{
    private readonly ITodoService _todoService;

    public SaleWidgetBinder(ITodoService todoService)
    {
        _todoService = todoService;
    }
    public (string value, Dictionary<string, Object> valueOptions) Bind(Dictionary<string, string> options)
    {
        var v = _todoService.GetTodoAsync(int.Parse(options["id"])).GetAwaiter().GetResult().CONTENTS;
        var result = _todoService.GetAllTodoAsync(Guid.Parse(options["guid"])).GetAwaiter().GetResult().Select(m => new
        {
            Key = m.NOTIFY_DT.Value.ToShortDateString(),
            Value = m.CONTENTS
        });
        var vo = new Dictionary<string, object>();
        result.xForEach(item =>
        {
            vo.Add(item.Key, item.Value);
        });
        return (v, vo);
    }

    // public string GetSale(int id)
    // {
    //     return _connection.xDatabaseTry<string>(connection =>
    //     {
    //         var result = connection.QueryFirst<string>("SELECT GETDATE()");
    //         return (string)result;
    //     });
    //
    //     // return connection.xDatabaseTryAsync<string>(async (connection) =>
    //     // {
    //     //     var result = await connection.QueryFirstAsync<string>("SELECT GETDATE()");
    //     //     return result;
    //     // }).GetAwaiter().GetResult();
    //
    //     return string.Empty;
    // }
    //
    // public Dictionary<string, string> GetSaleValueOptions(string comCode)
    // {
    //     return new (null, null);
    // }
}