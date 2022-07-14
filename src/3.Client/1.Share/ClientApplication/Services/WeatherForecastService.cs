using System.Net.Http.Json;
using ClientApplication.ViewModel;
using Domain.Base;
using Domain.Entities;
using Domain.Entities.WeatherForecast;
using Domain.Response;
using InjectionExtension;
using Newtonsoft.Json;

namespace ClientApplication.Services;

public interface IWeatherForecastService
{
    Task<List<TB_WEATHERFORECAST>> GetsForecastAsync();
}

[AddService(ENUM_LIFE_TIME_TYPE.Scope, typeOfInterface:typeof(IWeatherForecastService))]
public class WeatherForecastService : IWeatherForecastService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IContextBase _contextBase;
    public WeatherForecastService(IHttpClientFactory clientFactory,
        IContextBase contextBase)
    {
        this._clientFactory = clientFactory;
        this._contextBase = contextBase;
    }
    
    /// <summary>
    /// Gets the weather forecast. use the HttpClient to call the WebApiApplication.Server.
    /// </summary>
    public async Task<List<TB_WEATHERFORECAST>> GetsForecastAsync()
    {
        using var client = _clientFactory.CreateClient(ClientConst.CLIENT_NAME);
        var request =
            new HttpRequestMessage(HttpMethod.Get, ClientConst.GET_WEATHERFORECAST);
        request.Headers.Add("Bearer", _contextBase.AuthorizeInfo.AccessToken);
        var response = client.SendAsync(request).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();
        var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        var items = JsonConvert.DeserializeObject<ResultBase<List<TB_WEATHERFORECAST>>>(result);

        return items.Data;
    }

    // /// <summary>
    // /// Gets the weather forecast. use by the http client factory.
    // /// </summary>
    // public async Task<TB_WEATHERFORECAST[]> GetForecastUseByHttpClientFactoryAsync()
    // {
    //     var response = await _httpClient.GetAsync("api/v1/WeatherForecast");
    //     return await response.Content.ReadFromJsonAsync<TB_WEATHERFORECAST[]>();
    // }
}