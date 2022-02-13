using System;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Application.Request;
using Application.Response;
using eXtensionSharp;

namespace Application.Infrastructure.Message
{
    public class LineMessageInfo
    {
        [JsonPropertyName("messages")]
        public LineMessage[] Messages { get; set; }
    }

    public class LineMessage
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }        
    }
    
    public record LineNotifyMessageRequest(string AccessToken, string RetryKey, LineMessageInfo LineMessageInfo) : INotifyMessageRequest;
    
    public class LineNotifyMessageProvider : NotifyMessageProviderBase
    {
        public override async Task<IResult> SendMessageAsync(INotifyMessageRequest request)
        {
            var lineMessageRequest = request as LineNotifyMessageRequest;
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://api.line.me");
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {lineMessageRequest.AccessToken}");
                httpClient.DefaultRequestHeaders.Add("X-Line-Retry-Key", lineMessageRequest.RetryKey);
                var stringContent = new StringContent(lineMessageRequest.LineMessageInfo.xToJson(), Encoding.UTF8,
                    "application/json");
                var response = await httpClient.PostAsync("", stringContent);
                if (!response.IsSuccessStatusCode) return await Result.FailAsync(response.StatusCode.ToString());
                var result = await response.Content.ReadAsStringAsync();
                
                //your result code ...
                return await Result.SuccessAsync(result);
            }
        }

        public override object ConvertRequest(INotifyMessageRequest request)
        {
            throw new NotImplementedException();
        }
    }
}