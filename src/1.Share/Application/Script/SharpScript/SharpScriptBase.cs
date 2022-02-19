﻿namespace Application.Script.SharpScript;

public abstract class SharpScriptBase<TOptions, TRequest, TResult>
{
    public TOptions Options { get; set; }
    public TRequest Request { get; set; }
    public TResult Result { get; protected set; }
    public abstract void Execute();
    public abstract bool Validate(out string message);
}