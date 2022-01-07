using System.Reflection;

namespace Chloe.Reflection
{
    public class MemberMapperContainer
    {
        static readonly System.Collections.Concurrent.ConcurrentDictionary<MemberInfo, MemberMapper> Cache = new System.Collections.Concurrent.ConcurrentDictionary<MemberInfo, MemberMapper>();
        public static MemberMapper Get(MemberInfo memberInfo)
        {
            MemberMapper mapper = null;
            if (!Cache.TryGetValue(memberInfo, out mapper))
            {
                lock (memberInfo)
                {
                    if (!Cache.TryGetValue(memberInfo, out mapper))
                    {
                        mapper = DefaultDelegateFactory.Instance.CreateMapper(memberInfo);
                        Cache.GetOrAdd(memberInfo, mapper);
                    }
                }
            }

            return mapper;
        }
    }
}
