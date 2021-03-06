using System;
using System.Net.Http;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorWasmApplication;
using ClientApplication.Services;
using ClientApplication.ViewModel;
using Microsoft.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
//builder.RootComponents.Add<HeadOutlet>("head::after");

#region [Add application injection]

builder.Services.AddScoped<CounterStateViewModel>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<WeatherForecastService>();
#endregion

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
