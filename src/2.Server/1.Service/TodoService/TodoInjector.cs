using System.Reflection;
using Application.Abstract;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoService.Services;

namespace TodoService
{
    internal class TodoInjector : IServiceInjectionBase
    {
        public void Inject(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ITodoService, Services.TodoService>()
                .AddMediatR(Assembly.Load(this.GetType().Namespace));
        }
    }
    
    public static class TodoInjectorExtension
    {
        public static void AddTodoInjector(this IServiceCollection services)
        {
            var diCore = new ServiceLoader(new[]
            {
                new TodoInjector()
            }, services, null);
            diCore.Inject();
        }
    }     
}