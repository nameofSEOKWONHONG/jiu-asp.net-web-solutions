using Microsoft.Extensions.Logging;

namespace SpectreConsoleApplication.Menus.Abstract;

public abstract class ActionBase
{
    protected readonly ILogger _logger;
    protected readonly IHttpClientFactory _clientFactory;

    public ActionBase(ILogger logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
    }
}