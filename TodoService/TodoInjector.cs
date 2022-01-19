using System.Reflection;
using Application.Abstract;
using Application.Interfaces.Todo;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoApplication.Services;

namespace TodoApplication
{
    internal class TodoInjector : IDependencyInjectorBase
    {
        public void Inject(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ITodoService, TodoService>()
                .AddMediatR(Assembly.Load(this.GetType().Namespace));
        }
    }
    
    public static class TodoInjectorExtension
    {
        public static void AddTodoInjector(this IServiceCollection services)
        {
            var diCore = new DependencyInjector(new[]
            {
                new TodoInjector()
            }, services, null);
            diCore.Inject();
        }
    }     
}