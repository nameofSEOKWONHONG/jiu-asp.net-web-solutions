using System;
using System.IO;
using eXtensionSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApiApplication.Extensions
{
    /// <summary>
    /// Plugin 추가 확장 집합
    /// </summary>
    internal static class PluginExtensions
    {
        /// <summary>
        /// Plugin 추가 구현
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        internal static void AddPluginFiles(this IServiceCollection services, IConfiguration configuration)
        {
            var enable = bool.Parse(configuration["PluginEnable"]);
            if(!enable) return;
            
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