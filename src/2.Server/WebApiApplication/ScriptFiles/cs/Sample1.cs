using System.Collections.Generic;
using Application.Script.SharpScript;

public class Sample1 : SharpScriptBase<bool, string, Dictionary<string, string>>
{
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

    public override bool Validate(out string message)
    {
        message = string.Empty;
        return true;
    }
}