using Domain.Entities;
using Domain.Enums;
using eXtensionSharp;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using SpectreConsoleApplication.Menus.Abstract;
using SpectreConsoleApplication.Utils;

namespace SpectreConsoleApplication.Menus.Member;

public class MemberView : ViewBase
{
    private readonly MemberAction _action;
    public MemberView(ILogger<MemberView> logger,
        ISession session,
        MemberAction action) : base(logger, session)
    {
        _action = action;
    }

    public override void Show()
    {
        CONTINUE:
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
                user.MOBILE.xValue<string>(string.Empty),
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

        var result = AnsiConsole.Ask<bool>("exit : ", true);
        if (!result) goto CONTINUE;
    }
}
