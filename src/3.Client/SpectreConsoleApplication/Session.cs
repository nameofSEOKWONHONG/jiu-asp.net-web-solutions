using Application.Infrastructure.Injection;

namespace SpectreConsoleApplication;

public interface ISession
{
    string ACCESS_TOKEN { get; set; }
}

[ServiceLifeTime(ENUM_LIFE_TYPE.Singleton, typeof(ISession))]
public class Session : ISession
{
    private string _accessToken;

    public string ACCESS_TOKEN
    {
        get => _accessToken;
        set
        {
            _accessToken = value;
        }
    }
}