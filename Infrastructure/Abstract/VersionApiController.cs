using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Abstract
{
    [Route("api/v{version:apiVersion}/[controller]")]
    public abstract class VersionApiController<T> : ApiControllerBase<T>
    {

    }
}