using Etrx.API.Contracts.Contests;
using Etrx.API.Contracts.Problems;
using Etrx.Application.Services;
using Etrx.Domain.Interfaces.Services;
using Etrx.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Etrx.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContestsController : ControllerBase
    {
        private readonly IContestsService _contestsService;

        public ContestsController(IContestsService contestsService)
        {
            _contestsService = contestsService;
        }

        /*[HttpGet("GetAllContests")]
        public ActionResult<IEnumerable<ContestsResponse>> GetAllContests()
        {
            var contests = _contestsService.GetAllContests();

            var contestsResponse = contests.Select(c => new ContestsResponse(c.ContestId, c.Name, c.StartTime));

            return Ok(contestsResponse);
        }*/

        [HttpGet("GetContestsByPage")]
        public ActionResult<IEnumerable<ProblemsResponse>> GetContestsByPage([FromQuery] int page, int pageSize, bool gym)
        {
            var contests = _contestsService.GetAllContests()
                .Where(c => c.Gym == gym)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var contestsResponse = contests.Select(c => new ContestsResponse(c.ContestId, c.Name, c.StartTime));

            return Ok(contestsResponse);
        }
    }
}
