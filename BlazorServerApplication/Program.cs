using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using BlazorServerApplication.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddMudServices();
//builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddHttpClient<WeatherForecastService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5001");
});
builder.Services.AddSingleton<SampleService>();


//https://auth0.com/blog/what-is-blazor-tutorial-on-building-webapp-with-authentication/
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(options => {
    //https://stackoverflow.com/questions/66311898/asp-net-core-3-1-httpcontext-signoutasync-does-not-redirect
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
}).AddKakaoTalk(options =>
{
    options.ClientId = "[client key]";
    options.ClientSecret = "[client secret]";
    options.Events = new OAuthEvents()
    {
        OnTicketReceived = (context) =>
        {
            return Task.CompletedTask;
        },
        OnAccessDenied = context =>
        {
            return Task.CompletedTask;
        },
        OnCreatingTicket = context =>
        {
            Console.WriteLine(context.AccessToken);
            Console.WriteLine(context.RefreshToken);
            Console.WriteLine(context.Identity.AuthenticationType);
            Console.WriteLine(context.Identity.IsAuthenticated);
            var kakaoValueKind = JsonSerializer.Deserialize<KakaoAuthInfo>(context.User.GetRawText());
            Console.WriteLine(kakaoValueKind.Id);
            
            //convert ticket context to oauthinfo
            //save db oauthinfo;
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
