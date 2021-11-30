using System;
using System.IO;
using eXtensionSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApiApplication.Extensions
{
    internal static class PluginExtensions
    {
        internal static void AddPluginFiles(this IServiceCollection services, IConfiguration configuration)
        {
            var mvcBuilder = services.AddMvc();
            var pluginPath = Path.Combine(AppContext.BaseDirectory, "plugins");
            if (!Directory.Exists(pluginPath)) Directory.CreateDirectory(pluginPath);
            var dirs = Directory.GetDirectories(pluginPath);
            dirs.xForEach(dir =>
            {
                var file = Path.Combine(dir, Path.GetFileName(dir) + ".dll");
                mvcBuilder.AddPluginFromAssemblyFile(file);
            });
        }
    }
}