﻿namespace Chloe.Infrastructure.Interception
{
    public static class DbInterception
    {
        static volatile List<IDbCommandInterceptor> _interceptors = new List<IDbCommandInterceptor>();
        static readonly object _lockObject = new object();
        public static void Add(IDbCommandInterceptor interceptor)
        {
            PublicHelper.CheckNull(interceptor);

            lock (_lockObject)
            {
                List<IDbCommandInterceptor> newList = _interceptors.ToList();
                newList.Add(interceptor);
                newList.TrimExcess();
                _interceptors = newList;
            }
        }
        public static void Remove(IDbCommandInterceptor interceptor)
        {
            PublicHelper.CheckNull(interceptor);

            lock (_lockObject)
            {
                List<IDbCommandInterceptor> newList = _interceptors.ToList();
                newList.Remove(interceptor);
                newList.TrimExcess();
                _interceptors = newList;
            }
        }

        public static IDbCommandInterceptor[] GetInterceptors()
        {
            return _interceptors.ToArray();
        }
    }
}
