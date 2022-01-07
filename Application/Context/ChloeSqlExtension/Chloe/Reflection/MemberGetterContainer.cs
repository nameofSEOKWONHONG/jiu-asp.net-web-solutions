using System.Reflection;

namespace Chloe.Reflection
{
    public class MemberGetterContainer
    {
        static readonly System.Collections.Concurrent.ConcurrentDictionary<MemberInfo, MemberGetter> Cache = new System.Collections.Concurrent.ConcurrentDictionary<MemberInfo, MemberGetter>();
        public static MemberGetter Get(MemberInfo memberInfo)
        {
            MemberGetter getter = null;
            if (!Cache.TryGetValue(memberInfo, out getter))
            {
                lock (memberInfo)
                {
                    if (!Cache.TryGetValue(memberInfo, out getter))
                    {
                        getter = DefaultDelegateFactory.Instance.CreateGetter(memberInfo);
                        Cache.GetOrAdd(memberInfo, getter);
                    }
                }
            }

            return getter;
        }
    }
}
