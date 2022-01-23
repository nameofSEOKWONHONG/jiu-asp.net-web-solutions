using Domain.Enums;

namespace Domain.Configuration;

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