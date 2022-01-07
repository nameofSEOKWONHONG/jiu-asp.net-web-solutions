using System.Reflection;

namespace Chloe.Reflection
{
    public class MethodInvokerContainer
    {
        static readonly System.Collections.Concurrent.ConcurrentDictionary<MethodInfo, MethodInvoker> Cache = new System.Collections.Concurrent.ConcurrentDictionary<MethodInfo, MethodInvoker>();
        public static MethodInvoker Get(MethodInfo method)
        {
            MethodInvoker invoker = null;
            if (!Cache.TryGetValue(method, out invoker))
            {
                lock (method)
                {
                    if (!Cache.TryGetValue(method, out invoker))
                    {
                        invoker = DefaultDelegateFactory.Instance.CreateInvoker(method);
                        Cache.GetOrAdd(method, invoker);
                    }
                }
            }

            return invoker;
        }
    }
}
