using System.Reflection;

namespace Chloe.Reflection
{
    public class MemberSetterContainer
    {
        static readonly System.Collections.Concurrent.ConcurrentDictionary<MemberInfo, MemberSetter> Cache = new System.Collections.Concurrent.ConcurrentDictionary<MemberInfo, MemberSetter>();
        public static MemberSetter Get(MemberInfo memberInfo)
        {
            MemberSetter setter = null;
            if (!Cache.TryGetValue(memberInfo, out setter))
            {
                lock (memberInfo)
                {
                    if (!Cache.TryGetValue(memberInfo, out setter))
                    {
                        setter = DefaultDelegateFactory.Instance.CreateSetter(memberInfo);
                        Cache.GetOrAdd(memberInfo, setter);
                    }
                }
            }

            return setter;
        }
    }
}
