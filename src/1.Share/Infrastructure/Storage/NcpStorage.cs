using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Domain.Configuration;
using eXtensionSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Storage.Files;

public class NcpStorage : StorageProviderBase
{
    public NcpStorage(IOptionsSnapshot<StorageConfigOption> option):base(option)
    {
        
    }
    
    public override async Task<IEnumerable<StorageResult>> UploadAsync(IEnumerable<IFormFile> files)
    {
        // using (var formContent = new MultipartFormDataContent())
        // {
        //     // using var file = File.OpenRead(option.TempFileName);
        //     // var streamContent = new StreamContent(option.Stream);
        //     // streamContent.Headers.Add("Bearer", option.BearerKey);
        //     // streamContent.Headers.ContentType = new MediaTypeHeaderValue($"image/{option.Extension}");
        //     // formContent.Add(streamContent, name:"files", fileName:option.FileName.xGetHashCode());
        //     // if (option.Etc.xIsNotEmpty())
        //     // {
        //     //     formContent.Add(new StringContent(option.Etc.xToJson()));
        //     // }
        //     // using (var client = new HttpClient())
        //     // {
        //     //     //1sec per byte
        //     //     client.Timeout = TimeSpan.FromSeconds(option.Stream.Length);
        //     //     client.BaseAddress = new Uri("https://ncp.cloud.com");
        //     //     var res = await client.PostAsync("/storage/upload/", formContent);
        //     //     res.EnsureSuccessStatusCode();
        //     //     return (await res.Content.ReadAsStringAsync()).xToEntity<StorageResult>();
        //     // }
        // }
        return null;
    }

    public override ENUM_STORAGE_TYPE GetStorageType() => ENUM_STORAGE_TYPE.NCP;
}