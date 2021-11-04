using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazorServerApplication.Data;

namespace BlazorServerApplication.Services;

public class WeatherForecastService
{
    private readonly HttpClient _httpClient;
    public WeatherForecastService(HttpClient httpClient)
    {
        this._httpClient = httpClient;
    }
    
    /// <summary>
    /// Gets the weather forecast. use the HttpClient to call the WebApiApplication.Server.
    /// </summary>
    public async Task<WeatherForecast[]> GetForecastAsync()
    {
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri("https://localhost:5001");
            var response = await client.GetAsync("api/v1/WeatherForecast");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<WeatherForecast[]>();
                return result;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets the weather forecast. use by the http client factory.
    /// </summary>
    public async Task<WeatherForecast[]> GetForecastUseByHttpClientFactoryAsync()
    {
        var response = await _httpClient.GetAsync("api/v1/WeatherForecast");
        return await response.Content.ReadFromJsonAsync<WeatherForecast[]>();
    }
}
