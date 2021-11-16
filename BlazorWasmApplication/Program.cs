using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorWasmApplication;
using ClientApplication.Injector;
using ClientApplication.Service;
using ClientApplication.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Abstract;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

#region [Add application injection]
builder.Services.AddInjector();
#endregion

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
