using InjectionExtension;

namespace Domain.Base;

public interface IClientSession
{
    string AccessToken { get; set; }
}

[AddService(ENUM_LIFE_TIME_TYPE.Singleton, typeof(IClientSession))]
public class ClientSession : IClientSession
{
    public string AccessToken { get; set; }
}