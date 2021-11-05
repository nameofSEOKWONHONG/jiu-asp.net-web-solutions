using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using BlazorServerApplication.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApiApplication.SharedLibrary.Dtos;

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
            
            _logger.LogTrace(oauthInfo.ToString());
            
            return base.CreatingTicket(context);
        }

        public override Task TicketReceived(TicketReceivedContext context)
        {
            return base.TicketReceived(context);
        }
    }
}