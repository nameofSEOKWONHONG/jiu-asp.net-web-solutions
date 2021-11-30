using System.Reflection;
using Application.Abstract;
using Application.Interfaces.Todo;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoApplication.Services;

namespace TodoApplication
{
    internal class TodoApplicationInjector : IDependencyInjectorBase
    {
        public void Inject(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ITodoService, TodoService>();
            services.AddMediatR(Assembly.Load("TodoApplication"));
        }
    }
    
    public static class TodoApplicationInjectorExtension
    {
        public static void AddTodoApplicationInjector(this IServiceCollection services)
        {
            var diCore = new DependencyInjectorImpl(new[]
            {
                new TodoApplicationInjector()
            }, services, null);
        }
    }     
}