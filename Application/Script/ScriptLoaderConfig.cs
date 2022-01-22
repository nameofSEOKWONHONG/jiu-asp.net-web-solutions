using Domain.Enums;
using eXtensionSharp;

namespace Application.Script;

public class ScriptLoaderConfig
{
    public double Version { get; set; }
    public ScriptResetFileConfig[] ResetFileConfigs { get; set; }
}

public class ScriptResetFileConfig
{
    private ENUM_SCRIPT_TYPE _type;

    public ENUM_SCRIPT_TYPE ScriptType
    {
        get => _type;
        set
        {
            this._type = value;
        }
    }
    public string[] ResetFiles { get; set; }
}