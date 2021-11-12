using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApiApplication.Dtos;
using WebApiApplication.SharedLibrary.Dtos;
using WebApiApplication.SharedLibrary.Entities;

namespace BlazorServerApplication.Services
{
    public class KakaoOAuthEventService : OAuthEvents
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        public KakaoOAuthEventService(IServiceProvider serviceProvider)
        {
            this._logger = serviceProvider.GetService<ILogger<KakaoOAuthEventService>>();
            this._serviceProvider = serviceProvider;
        }
        
        public override Task CreatingTicket(OAuthCreatingTicketContext context)
        {
            var oauthInfo = new OAuthInfo<KakaoAuthInfo>(context.AccessToken, context.RefreshToken,
                context.Identity.AuthenticationType, context.Identity.IsAuthenticated);
            
            _logger.LogTrace(oauthInfo.ToString());
            _logger.LogTrace(context.User.GetRawText());

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://kapi.kakao.com");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {context.AccessToken}");
                var response = client.GetAsync("/v2/user/me?secure_resource=true").GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    var kakaoAuthInfo = response.Content.ReadFromJsonAsync<KakaoAuthInfo>().GetAwaiter().GetResult();
                    oauthInfo.AuthEntity = kakaoAuthInfo;
                }
            }

            if (oauthInfo.IsAuthenticated)
            {
                //if login info exists
                var user = new User();
                if (user != null)
                {
                    var httpContext = _serviceProvider.GetService<IHttpContextAccessor>();
                    var cookieOptions = new CookieOptions()
                    {
                        Expires = DateTime.Now.AddHours(12),
                        SameSite = SameSiteMode.Strict,
                        Secure = true,
                    };
                    httpContext.HttpContext.Response.Cookies.Append("user", JsonSerializer.Serialize(user), cookieOptions);                    
                }
                else
                {
                    //save auth info
                    using (var client = new HttpClient())
                    {
                        var postData = new UserRequest()
                        {
                            Email = oauthInfo.AuthEntity.KakaoAccount.Email,
                            Password = "[systemdefaultpassword]"
                        };
                        client.BaseAddress = new Uri("https://localhost:5001");
                        var respoonse = client.PostAsync("/api/v1/Auth/SingUp", new StringContent(JsonSerializer.Serialize(postData))).GetAwaiter().GetResult();
                        if (respoonse.IsSuccessStatusCode)
                        {
                            //write mail check
                        }
                    }                    
                }
            }
            
            _logger.LogTrace(oauthInfo.ToString());
            
            return base.CreatingTicket(context);
        }

        public override Task TicketReceived(TicketReceivedContext context)
        {
            return base.TicketReceived(context);
        }
    }
}