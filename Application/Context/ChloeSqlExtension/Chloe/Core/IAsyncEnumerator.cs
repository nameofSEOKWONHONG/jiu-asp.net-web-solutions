#if netfx
using System;

namespace System.Collections.Generic
{
    internal interface IAsyncEnumerator<out T> : IDisposable
    {
        T Current { get; }

        BoolResultTask MoveNextAsync();
    }
}
#endif
