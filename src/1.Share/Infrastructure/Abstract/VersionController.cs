using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Abstract
{
    [Route("api/v{version:apiVersion}/[controller]")]
    public abstract class VersionController<TController> : ApiControllerBase<TController>
    {

    }
}