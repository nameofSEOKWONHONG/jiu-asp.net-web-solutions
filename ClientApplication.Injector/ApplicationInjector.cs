using System.Collections.Generic;
using ClientApplication.Service;
using ClientApplication.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Application.Abstract;

namespace ClientApplication.Injector
{
    public static class ApplicationInjector 
    {
        public static void AddInjector(this IServiceCollection services)
        {
            var injectors = new List<IDependencyInjectorBase>()
            {
                new ServiceDependencyInjector(),
                new ViewModelDependencyInjector()
            };
            
            injectors.ForEach(injector =>
            {
                injector.Inject(services, null);
            });
        }
    }    
}

