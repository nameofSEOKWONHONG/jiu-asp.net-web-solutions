/*Script Syntax Reference : https://github.com/oleg-shilo/cs-script/wiki/Script-Syntax*/

//css_import ScriptFiles/cs/Sample1.cs, preserve_main;

using System.Linq;
using System.Threading.Tasks;
using Application.Context;
using Application.Script.SharpScript;
using eXtensionSharp;
using Infrastructure.Persistence;

/// <summary>
/// 캐시 전 : 0.2~3초
/// 캐시 후 : 0.1~2초
/// 비스크립트 : 0.1초내
/// </summary>
public class HelloWorldScript : SharpScriptBase<ApplicationDbContext, string, string>
{
    private readonly Sample1 _sample1;
    public HelloWorldScript()
    {
        _sample1 = new Sample1();
        _sample1.Options = false;
        _sample1.Request = string.Empty;
    }

    public override Task ExecuteAsync()
    {
        throw new System.NotImplementedException();
    }

    public override void Execute()
    {
        var user = this.Options.Users.FirstOrDefault();
        var result = $"Hello World, Request : {this.Request}, dbContext : {user.EMAIL}";
        _sample1.Options = false;
        _sample1.Request = result;
        _sample1.Execute();
        this.Result =
            $"result : {result}, sample1 result keys : {string.Join("|", _sample1.Result.Keys)}, sample1 result values : {string.Join("|", _sample1.Result.Values)}";
    }
}

