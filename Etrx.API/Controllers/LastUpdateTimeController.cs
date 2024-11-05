using Etrx.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Etrx.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LastUpdateTimeController : ControllerBase
    {
        private readonly ILastUpdateTimeService _lastUpdateTimeService;
        public LastUpdateTimeController(ILastUpdateTimeService lastTimeUpdateService)
        {
            _lastUpdateTimeService = lastTimeUpdateService;
        }

        [HttpGet("GetLastUpdateTime")]
        public ActionResult<DateTime> GetLastUpdateTime([FromQuery] string tableName)
        {
            try
            {
                DateTime time = _lastUpdateTimeService.GetLastUpdateTime(tableName);
                return Ok(time);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}