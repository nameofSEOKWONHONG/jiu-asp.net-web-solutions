using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Script.SharpScript;

public class Sample1 : SharpScriptBase<bool, string, Dictionary<string, string>>
{
    public override Task ExecuteAsync()
    {
        throw new System.NotImplementedException();
    }

    public override void Execute()
    {
        this.Result = new Dictionary<string, string>()
        {
            {"A", "1"},
            {"B", "2"},
            {"C", "3"},
            {"D", "4"},
        };
    }
}