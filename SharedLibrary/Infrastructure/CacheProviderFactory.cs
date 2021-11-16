using System;
using Microsoft.Extensions.Caching.Memory;
using SharedLibrary.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace SharedLibrary.Infrastructure
{
    public class CacheProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public CacheProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ICacheProvider Create<T>() where T : ICacheProvider
        {
            return _serviceProvider.GetService<T>();
        }
    }
}