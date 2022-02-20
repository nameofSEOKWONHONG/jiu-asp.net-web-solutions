using Domain.Entities.WeatherForecast;
using Domain.Response;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SpectreConsoleApplication.Menus.Abstract;

namespace SpectreConsoleApplication.Menus.WeatherForecast;

public sealed class WeatherForecastAction : ActionBase
{
    public WeatherForecastAction(ILogger<WeatherForecastAction> logger,
        IHttpClientFactory clientFactory) : base(logger, clientFactory)
    {
    }

    public IEnumerable<TB_WEATHERFORECAST> GetWeatherForecastList()
    {
        using var client = _clientFactory.CreateClient();
        var request =
            new HttpRequestMessage(HttpMethod.Get, "https://localhost:5001/api/WeatherForecast?api-version=1");
        request.Headers.Add("Bearer",  AppConst.AccessToken);
        var response = client.SendAsync(request).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();
        var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        var items = JsonConvert.DeserializeObject<Result<TB_WEATHERFORECAST[]>>(result);
        return items.Data;
    }
}