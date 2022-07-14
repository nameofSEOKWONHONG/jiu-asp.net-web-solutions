using Domain.Base;
using Domain.Entities;
using Domain.Enums;
using eXtensionSharp;
using InjectionExtension;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using SpectreConsoleApplication.Menus.Abstract;
using SpectreConsoleApplication.Utils;

namespace SpectreConsoleApplication.Menus.Member;

[AddService(ENUM_LIFE_TIME_TYPE.Singleton)]
public class MemberView : ViewBase
{
    private readonly IMemberAction _action;
    public MemberView(ILogger<MemberView> logger,
        IContextBase contextBase,
        IMemberAction action) : base(logger, contextBase)
    {
        _action = action;
    }

    public override void Show()
    {
        var members = _action.GetMembers();
        if (members.xIsEmpty())
        {
            AnsiConsole.Ask<string>("data is empty, exit", "Y");
            return;
        }
        
        TableUtil.TableDraw<TB_USER>(members, (table, user) =>
        {
            table.AddRow(new string[]
            {
                user.EMAIL.xValue(string.Empty), 
                user.PASSWORD.xValue(string.Empty), 
                user.PHONE_NUM.xValue<string>(string.Empty),
                user.ACTIVE_USER_YN.xValue<string>(string.Empty), 
                user.AUTO_CONFIRM_EMAIL_YN.xValue<string>(string.Empty),
                user.ROLE.ROLE_TYPE.xValue(ENUM_ROLE_TYPE.GUEST), 
                user.ID.xValue<string>(string.Empty),
                user.WRITE_ID.xValue(string.Empty), 
                user.WRITE_DT.xValue(ENUM_DATE_FORMAT.YYYY_MM_DD_HH_MM_SS),
                user.UPDATE_ID.xValue(string.Empty), 
                user.UPDATE_DT?.xValue(ENUM_DATE_FORMAT.YYYY_MM_DD_HH_MM_SS) ?? string.Empty
            });
        });
    }
}
