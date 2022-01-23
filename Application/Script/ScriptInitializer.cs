using System;
using System.IO;
using eXtensionSharp;

namespace Application.Script;

/// <summary>
/// 스크립트 초기화 Reset
/// </summary>
public class ScriptInitializer
{
    public ScriptInitializer()
    {
    }

    public bool Reset(IScriptReset scriptReset, double version, string[] resetFiles = null)
    {
        var isReset = true;
        
        if (!version.xIsEquals(scriptReset.Version))
        {
            isReset = scriptReset.Reset(null);    
        }        
        else if (version.xIsEquals(scriptReset.Version))
        {
            resetFiles.xForEach(file =>
            {
                if (file.xIsEmpty()) return false;
                var fileNamePath = Path.Combine(AppContext.BaseDirectory, file);
                isReset = scriptReset.Reset(fileNamePath);
                if (isReset.xIsFalse())
                    throw new Exception($"{nameof(ScriptInitializer)} : init is failed. version is {version}, script version : {scriptReset.Version}, filename is {file}");

                return true;
            });
        }

        scriptReset.Version = version;

        return isReset;
    }
}