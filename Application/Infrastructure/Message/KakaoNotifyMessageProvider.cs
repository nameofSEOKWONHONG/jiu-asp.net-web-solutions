using System;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Application.Request;
using Application.Response;
using eXtensionSharp;
using RestSharp;

namespace Application.Infrastructure.Message
{
    public class KakaoTalkSendInfo
    {
        [JsonPropertyName("receiver_uuids")]
        public string[] ReceiverUUIDList { get; set; }
        [JsonPropertyName("template_object")]
        public KakaoTalkTemplateObject TemplateObject { get; set; }
    }
    
    public class KakaoTalkTemplateObject
    {
        [JsonPropertyName("object_type")]
        public string ObjectType { get; set; } = "text";
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("link")]
        public KakaoTalkLink Link { get; set; }
    }
    
    public class KakaoTalkLink
    {
        [JsonPropertyName("web_url")]
        public string WebUrl { get; set; }

        [JsonPropertyName("mobile_web_url")]
        public string MobileWebUrl { get; set; }
    }
    
    public record KakaoTalkNotifyMessageRequest(string accessToken, KakaoTalkSendInfo kakaoTalkSendInfo) : INotifyMessageRequest;
    
    public class KakaoNotifyMessageProvider : NotifyMessageProviderBase
    {
        public KakaoNotifyMessageProvider()
        {
            
        }
        
        public override async Task<IResult> SendMessageAsync(INotifyMessageRequest request)
        {
            var convertedRequest = request as KakaoTalkNotifyMessageRequest;

            #region [use restclient]

            var url = "https://kapi.kakao.com/v1/api/talk/friends/message/default/send";
            var client = new RestClient(url);
            client.AddDefaultHeader("Authorization", $"Bearer {convertedRequest.accessToken}");
            var clientRequest = new RestRequest();
            clientRequest.Method = Method.POST;
            clientRequest.AddHeader("Accept", "application/json");
            clientRequest.Parameters.Clear();
            clientRequest.AddParameter("application/json", convertedRequest.kakaoTalkSendInfo.xToJson(),
                ParameterType.RequestBody);
            var response = client.Execute(clientRequest);
            var content = response.Content;            

            #endregion


            #region [use httpclient]
            
            // using (var httpclient = new HttpClient())
            // {
            //     httpclient.BaseAddress = new Uri("https://kapi.kakao.com");
            //     httpclient.DefaultRequestHeaders.Clear();
            //     httpclient.DefaultRequestHeaders.Add("Authorization", $"Bearer {convertedRequest.accessToken}");
            //     var postContent = new StringContent(convertedRequest.kakaoTalkSendInfo.xToJson(), Encoding.UTF8, "application/json");
            //     var responseMessage = await httpclient.PostAsync("/v1/api/talk/friends/message/default/send", postContent);
            //     if (responseMessage.IsSuccessStatusCode)
            //     {
            //         var responseContent = await responseMessage.Content.ReadAsStringAsync();
            //         var result = responseContent.xToEntity<object>();
            //     }
            // }

            #endregion
            
            //your result code ...
            return Result.Success(nameof(EmailNotifyMessageProvider));
        }

        public override object ConvertRequest(INotifyMessageRequest request)
        {
            throw new NotImplementedException();
        }
    }
}