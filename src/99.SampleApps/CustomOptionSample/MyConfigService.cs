using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;

namespace CustomOptionSample;

public class MyConfigService
{
    private readonly IOptionsMonitor<MyConfig> _optionsMonitor;
    private MyConfig _myConfig;
    public MyConfigService(IOptionsMonitor<MyConfig> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;
        _myConfig = _optionsMonitor.CurrentValue;
        _optionsMonitor.OnChange((args) =>
        {
            this._myConfig = args;
        });
    }

    public MyConfig Get()
    {
        return _myConfig;
    }
}