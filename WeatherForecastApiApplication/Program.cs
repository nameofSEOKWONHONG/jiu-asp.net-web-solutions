using Application.Infrastructure.Cache;
using Application.Infrastructure.Message;
using HelloWorldApplication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeatherForecastApplication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCacheProviderInject();
builder.Services.AddMessageProviderInject();
builder.Services.AddWeatherForecastApplicationInjector();
builder.Services.AddWeatherForecastApplicationCQRSInjector();
builder.Services.AddHelloWorldApplicationInjector();
builder.Services.AddHelloWorldApplicationCQRSInjector();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();