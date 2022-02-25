using System;
using System.IO;
using Application.Script.ClearScript;
using Application.Script.JavaScriptEngines.NodeJS;
using Application.Script.JintScript;
using Application.Script.PyScript;
using Application.Script.SharpScript;
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

    public bool Reset(IScriptLoaderBase scriptLoaderBase, double version, string[] resetFiles = null)
    {
        var isReset = true;
        
        if (!version.xIsEquals(scriptLoaderBase.Version))
        {
            isReset = scriptLoaderBase.Reset(null);    
        }        
        else if (version.xIsEquals(scriptLoaderBase.Version))
        {
            resetFiles.xForEach(file =>
            {
                if (file.xIsEmpty()) return false;
                var fullPath = file.xGetFileNameWithBaseDir();
                isReset = scriptLoaderBase.Reset(fullPath);
                if (isReset.xIsFalse())
                    throw new Exception($"{nameof(ScriptInitializer)} : init is failed. version is {version}, script version : {scriptLoaderBase.Version}, filename is {file}");

                return true;
            });
        }

        scriptLoaderBase.Version = version;

        return isReset;
    }
}