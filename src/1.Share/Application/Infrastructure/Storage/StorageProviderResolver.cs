using System;
using System.Collections.Generic;
using eXtensionSharp;
using InjectionExtension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Infrastructure.Files;

public delegate IStorageProvider StorageProviderResolver(ENUM_STORAGE_TYPE type);
    
internal class StorageProviderInjector : IServiceInjectionBase
{
    private readonly Dictionary<ENUM_STORAGE_TYPE, Func<IServiceProvider, IStorageProvider>>
        _notifyState =
            new ()
            {
                { ENUM_STORAGE_TYPE.NCP, (s) => s.GetRequiredService<NcpStorage>() },
            };
            
    public void Inject(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<NcpStorage>()
            .AddSingleton<StorageProviderResolver>(provider => key =>
            {
                var func = _notifyState[key];
                if (func.xIsEmpty()) throw new NotImplementedException($"key {key.ToString()} not implemented");
                return func(provider);
            });
    }
}