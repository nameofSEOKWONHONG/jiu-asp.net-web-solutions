using eXtensionSharp;

namespace Application.Script;

public class ScriptLoaderConfig
{
    public double Version { get; set; }
    public ScriptResetFileConfig[] ResetFileConfigs { get; set; }
}

public class ScriptResetFileConfig
{
    public string ScriptType { get; set; }
    public string[] ResetFiles { get; set; }
}

public class ENUM_SCRIPT_TYPE : XEnumBase<ENUM_SCRIPT_TYPE>
{
    public static readonly ENUM_SCRIPT_TYPE CSHARP = Define("CSHARP");
    public static readonly ENUM_SCRIPT_TYPE JS = Define("JS");
    public static readonly ENUM_SCRIPT_TYPE PY = Define("PY");
}