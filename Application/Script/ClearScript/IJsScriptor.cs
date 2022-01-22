using System;

namespace Application.Script.ClearScript;

public interface IJsScriptor
{
    void Execute<TRequest>(TRequest request, Action<object> action);
}