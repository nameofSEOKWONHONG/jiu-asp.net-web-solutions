
using System.Linq;
using Application.Context;
using Application.Script.CsScript;
using eXtensionSharp;

/// <summary>
/// 캐시 전 : 0.2~3초
/// 캐시 후 : 0.1~2초
/// 비스크립트 : 0.1초내
/// </summary>
public class HelloWorldScript : CsScriptBase<JIUDbContext, string, string>
{
    public HelloWorldScript()
    {
        
    }
    
    public override void Execute()
    {
        var user = this.Options.Users.FirstOrDefault();
        this.Result = $"Hello World : {this.Request} {user.EMAIL}";
    }

    public override bool Validate(out string message)
    {
        var valid = true;
        message = string.Empty;
        
        valid = this.Options.xIsNotEmpty();
        if (valid.xIsFalse())
        {
            message = $"valid : {valid}, options is null";
            return valid;
        }
        
        valid = this.Request.xIsNotEmpty();
        if (valid.xIsFalse())
        {
            message = $"valid : {valid}, request is null";
            return valid;
        }
        
        return valid;
    }
}