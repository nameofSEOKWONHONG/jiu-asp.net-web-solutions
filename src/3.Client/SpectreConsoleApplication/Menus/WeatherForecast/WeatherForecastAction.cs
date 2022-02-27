using Domain.Entities.WeatherForecast;
using Domain.Response;
using eXtensionSharp;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SpectreConsoleApplication.Menus.Abstract;

namespace SpectreConsoleApplication.Menus.WeatherForecast;

public sealed class WeatherForecastAction : ActionBase
{
    private IEnumerable<TB_WEATHERFORECAST> _items;
    public IEnumerable<TB_WEATHERFORECAST> Items
    {
        get
        {
            _items.xIfEmpty(() =>
            {
                using var client = _clientFactory.CreateClient(AppConst.HTTP_NAME);
                var request =
                    new HttpRequestMessage(HttpMethod.Get, "api/WeatherForecast?api-version=1");
                request.Headers.Add("Bearer", AppConst.ACESS_TOKEN);
                var response = client.SendAsync(request).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();
                var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var items = JsonConvert.DeserializeObject<ResultBase<TB_WEATHERFORECAST[]>>(result);
                _items = items.Data;
            });

            return _items;
        }
    }
    
    public TB_WEATHERFORECAST SelectedItem { get; set; }
    
    public WeatherForecastAction(ILogger<WeatherForecastAction> logger,
        IHttpClientFactory clientFactory) : base(logger, clientFactory)
    {
    }

    public bool Save()
    {
        if (SelectedItem.xIsEmpty()) throw new Exception("selected item is null");
        //insert or update
        SelectedItem = null;
        return true;
    }

    public override void Dispose()
    {
        Console.WriteLine($"{nameof(WeatherForecastAction)} dispose");
    }
}