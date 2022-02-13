using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// TODO : 인증 및 로드밸런스 구현하기
/*
 * Proxy Server로 처리할 수 있는 것 (역활)
 * 1) 인증 Authroized
 * 2) 권한 Permission
 * 3) 로드밸런스
 * 4) 보안
 * -> WebApiApplication에 구현한 인증 및 권한 설정을 여기서 처리할 수 있다.
 * -> json으로 설정하는 ReverseProxy 역활을 코드로 대체하여 로드밸런스 역활을 부여할 수 있다.
 * -> 실제 라우팅되는 URL이 서버가 아닌 Proxy 서버로 향하므로 보안을 더 할 수 있다.
 */ 
// reference : https://microsoft.github.io/reverse-proxy/
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapReverseProxy();
});

app.Run();