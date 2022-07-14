using System.Net.Http.Headers;
using ClientApplication.Services;
using Domain.Base;
using Domain.Entities;
using InjectionExtension;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SpectreConsoleApplication.Menus.Abstract;

namespace SpectreConsoleApplication.Menus.Member;

public interface IMemberAction
{
    IEnumerable<TB_USER> GetMembers();
}

[AddService(ENUM_LIFE_TIME_TYPE.Scope, typeof(IMemberAction))]
public class MemberAction : ActionBase, IMemberAction
{
    private readonly IMemberService _memberService;
    public MemberAction(ILogger<MemberAction> logger, IContextBase contextBase, IHttpClientFactory clientFactory, IMemberService memberService) : base(logger, contextBase, clientFactory)
    {
        _memberService = memberService;
    }

    public IEnumerable<TB_USER> GetMembers()
    {
        return _memberService.GetMembersAsync().GetAwaiter().GetResult();
    }
}