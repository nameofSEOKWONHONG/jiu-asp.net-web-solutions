using Microsoft.AspNetCore.Http;

namespace Infrastructure.Middlewares;

public sealed class CustomHeaderMiddleware
{ 
    private readonly RequestDelegate _next;

    public CustomHeaderMiddleware(RequestDelegate next)
    {
        this._next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task Invoke(HttpContext context)
    {
        context.Response.Headers.Add("x-my-custom-header", "middelware response");
        await this._next(context);
    } 
}