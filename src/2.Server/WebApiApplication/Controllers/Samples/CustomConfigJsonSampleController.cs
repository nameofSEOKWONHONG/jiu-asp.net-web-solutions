using eXtensionSharp;
using Infrastructure.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace WebApiApplication.Controllers;

public class CustomConfigJsonSampleController : ApiControllerBase<CustomConfigJsonSampleController>
{
    private readonly FilterOption _filterOption;
    public CustomConfigJsonSampleController(IOptionsMonitor<FilterOption> filterOption)
    {
        _filterOption = filterOption.CurrentValue;
    }

    [HttpGet("GetFilters")]
    public IActionResult GetFilters()
    {
        return Ok(_filterOption.xToJson());
    }
}