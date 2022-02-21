using Domain.Entities;
using Domain.Enums;
using eXtensionSharp;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using SpectreConsoleApplication.Menus.Abstract;

namespace SpectreConsoleApplication.Menus.Member;

public class MemberView : ViewBase
{
    private readonly MemberAction _action;
    public MemberView(ILogger<MemberView> logger,
        MemberAction action) : base(logger)
    {
        _action = action;
    }

    public override void Show()
    {
        CONTINUE:
        var members = _action.GetMembers();
        var table = new Table();
        AnsiConsole.Live(table)
            .AutoClear(false)
            .Overflow(VerticalOverflow.Ellipsis)
            .Cropping(VerticalOverflowCropping.Top)
            .Start(ctx =>
            {
                var columns = new List<string>();
                typeof(TB_USER).GetProperties().xForEach(prop =>
                {
                    columns.Add(prop.Name);
                });
                table.AddColumns(columns.ToArray());
                ctx.Refresh();
                
                members.xForEach(member =>
                {
                    var map = member.xToDictionary();
                    var row = new List<string>();
                    columns.xForEach(column =>
                    {
                        row.Add(map[column].xValue<string>());
                    });
                    table.AddRow(row.ToArray());
                    ctx.Refresh();
                    
                    // table.AddRow(new string[]
                    // {
                    //     member.EMAIL.xValue(string.Empty), 
                    //     member.PASSWORD.xValue(string.Empty), 
                    //     member.PHONE_NUM.xValue<string>(string.Empty),
                    //     member.ACTIVE_USER_YN.xValue<string>(string.Empty), 
                    //     member.AUTO_CONFIRM_EMAIL_YN.xValue<string>(string.Empty),
                    //     member.ROLE.ROLE_TYPE.xValue(ENUM_ROLE_TYPE.GUEST), 
                    //     member.ID.xValue<string>(string.Empty),
                    //     member.WRITE_ID.xValue(string.Empty), 
                    //     member.WRITE_DT.xValue(ENUM_DATE_FORMAT.YYYY_MM_DD_HH_MM_SS),
                    //     member.UPDATE_ID.xValue(string.Empty), 
                    //     member.UPDATE_DT?.xValue(ENUM_DATE_FORMAT.YYYY_MM_DD_HH_MM_SS) ?? string.Empty
                    // });
                });
                // members.xForEach((item, i) =>
                // {
                //     var kv = item.xToDictionary();
                //     if (i == 0) 
                //     {
                //         table.AddColumns(kv.Keys.ToArray());
                //         
                //     }
                //
                //     table.AddRow(kv.Values.ToArray());
                //     ctx.Refresh();
                // });
            });

        var result = AnsiConsole.Ask<bool>("exit : ", true);
        if (!result) goto CONTINUE;
    }
}
