using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using eXtensionSharp;

namespace Application.Infrastructure.Files;

public class NcpStorage : StorageProviderBase
{
    public override async Task<StorageResult> UploadAsync(StorageOption option)
    {
        using (var formContent = new MultipartFormDataContent())
        {
            var streamContent = new StreamContent(option.Stream);
            streamContent.Headers.Add("Bearer", option.BearerKey);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue($"image/{option.Extension}");
            formContent.Add(streamContent, name:"files", fileName:option.FileName);
            if (option.Etc.xIsNotEmpty())
            {
                formContent.Add(new StringContent(option.Etc.xToJson()));
            }
            using (var client = new HttpClient())
            {
                //1sec per byte
                client.Timeout = TimeSpan.FromSeconds(option.Stream.Length);
                client.BaseAddress = new Uri("https://ncp.cloud.com");
                var res = await client.PostAsync("/storage/upload/", formContent);
                res.EnsureSuccessStatusCode();
                return (await res.Content.ReadAsStringAsync()).xToEntity<StorageResult>();
            }
        }
    }
}