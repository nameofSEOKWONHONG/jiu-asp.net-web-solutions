using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorWasmApplication;
using ClientApplication.Abstract;
using ClientApplication.Service;
using ClientApplication.ViewModel;
using Microsoft.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var injectors = new List<InjectorBase>
{
    new ViewModelInjector(),
    new ServiceInjector()
};

injectors.ForEach(injector =>
{
    injector.Inject(builder.Services);
});

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
