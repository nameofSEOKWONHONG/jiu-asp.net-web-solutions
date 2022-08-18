using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;

namespace CustomOptionSample;

/// <summary>
/// IOptionMonitor가 변경된 시점에 Callback을 통해 변경사항을 확인 할 수 있다.
/// IOptionSnapShot은 변경된 설정에 대하여 결과를 확인할 수 있다.
/// IOption은 최초 App실행시 설정된 설정값만을 확인한다.
/// </summary>
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