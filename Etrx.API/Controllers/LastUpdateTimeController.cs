using Etrx.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Etrx.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LastUpdateTimeController : ControllerBase
{
    private readonly ILastUpdateTimeService _lastUpdateTimeService;
    public LastUpdateTimeController(ILastUpdateTimeService lastTimeUpdateService)
    {
        _lastUpdateTimeService = lastTimeUpdateService;
    }

    [HttpGet("{tableName}")]
    public ActionResult<DateTime> GetLastUpdateTime(string tableName)
    {
        return Ok(_lastUpdateTimeService.GetLastUpdateTime(tableName));
    }
}