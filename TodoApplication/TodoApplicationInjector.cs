using System.Reflection;
using Application.Abstract;
using Application.Interfaces.Todo;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TodoApplication.Services;

namespace TodoApplication
{
    public class TodoApplicationInjector : DependencyInjectorBase
    {
        public override void Inject(IServiceCollection services)
        {
            services.AddTransient<ITodoService, TodoService>();
        }
    }
    
    public class TodoApplicationCQRSInjector : CQRSInjectorBase
    {
        public override Assembly GetAssembly()
        {
            return Assembly.Load("TodoApplication");
        }
    }
    
    public static class TodoApplicationInjectorExtension
    {
        public static void AddTodoApplicationInjector(this IServiceCollection services)
        {
            var injector = new TodoApplicationInjector();
            injector.Inject(services);
        }

        public static void AddTodoApplicationCQRSInjector(this IServiceCollection services)
        {
            var injector = new TodoApplicationCQRSInjector();
            services.AddMediatR(injector.GetAssembly());
        }
    }     
}