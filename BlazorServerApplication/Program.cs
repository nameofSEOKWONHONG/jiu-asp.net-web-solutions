using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MudBlazor.Services;

using BlazorServerApplication.Data;

using ClientApplication.Services;

var builder = WebApplication.CreateBuilder(args);

#region [Add services to the container.]
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
#endregion

#region [Add mud-blazor services to the container.]
builder.Services.AddMudServices();
#endregion

#region [add http services to the container.]
//builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddHttpClient<WeatherForecastService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5001");
});
builder.Services.AddSingleton<LoginCheckService>();
#endregion

#region [add oauth services to the container.]
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
    var provider = builder.Services.BuildServiceProvider();
    options.ClientId = "[key]";
    options.ClientSecret = "[secret]";
    options.Events = new KakaoOAuthEventService(provider);
});
#endregion

#region [add httpcontext accessor to the container.]
builder.Services.AddHttpContextAccessor();
#endregion

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