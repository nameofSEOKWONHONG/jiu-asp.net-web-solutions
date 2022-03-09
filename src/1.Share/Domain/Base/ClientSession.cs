using InjectionExtension;

namespace Domain.Base;

public interface IClientSession
{
    string AccessToken { get; set; }
}

[ServiceLifeTime(ENUM_LIFE_TYPE.Singleton, typeof(IClientSession))]
public class ClientSession : IClientSession
{
    public string AccessToken { get; set; }
}