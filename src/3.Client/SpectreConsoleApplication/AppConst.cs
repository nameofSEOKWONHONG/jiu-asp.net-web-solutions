namespace SpectreConsoleApplication;

public class AppConst
{
    public const string HTTP_NAME = "local";
    public const string HTTP_URL = "https://localhost:5001";
}

public interface ISession
{
    string ACCESS_TOKEN { get; set; }
}

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