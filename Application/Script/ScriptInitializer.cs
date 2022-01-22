using System;
using System.IO;
using Application.Script.CsScript;
using eXtensionSharp;

namespace Application.Script;

public class ScriptInitializer
{
    public ScriptInitializer()
    {
    }

    public bool Reset(IScriptLoader scriptLoader, double version, string[] resetFiles = null)
    {
        var isReset = true;
        
        if (!version.xIsEquals(scriptLoader.Version))
        {
            isReset = scriptLoader.Reset(null);    
        }        
        else if (version.xIsEquals(scriptLoader.Version))
        {
            resetFiles.xForEach(file =>
            {
                if (file.xIsEmpty()) return false;
                var fileNamePath = Path.Combine(AppContext.BaseDirectory, file);
                isReset = scriptLoader.Reset(fileNamePath);
                if (isReset.xIsFalse())
                    throw new Exception($"{nameof(ScriptInitializer)} : init is failed. version is {version}, script version : {scriptLoader.Version}, filename is {file}");

                return true;
            });
        }

        scriptLoader.Version = version;

        return isReset;
    }
}