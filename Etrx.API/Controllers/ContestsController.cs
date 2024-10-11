using Etrx.API.Contracts.Contests;
using Etrx.API.Contracts.Problems;
using Etrx.Application.Services;
using Etrx.Domain.Interfaces.Services;
using Etrx.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

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

        [HttpGet("GetAllContests")]
        public ActionResult<IEnumerable<ContestsResponse>> GetAllContests()
        {
            var contests = _contestsService.GetAllContests();

            var contestsResponse = contests.Select(c => new ContestsResponse(c.ContestId, c.Name, c.StartTime));

            return Ok(contestsResponse);
        }

        [HttpGet("GetContestById")]
        public ActionResult<ContestsResponse> GetContestById([FromQuery] int contestId)
        {
            var contest = _contestsService.GetContestById(contestId);

            if (contest != null)
            {
                var response = new ContestsResponse(contest.ContestId, contest.Name, contest.StartTime);
                return Ok(response);
            }
            else
            {
                return NotFound($"Contest {contestId} not fount");
            }
        }

        [HttpGet("GetContestsByPageWithSort")]
        public ActionResult<ContestsWithPropsResponse> GetContestsByPage([FromQuery] int page, int pageSize, bool? gym, string? sortByProp, bool? sortOrder)
        {
            sortOrder ??= true;
            sortByProp ??= "contestid";
            
            var contests = _contestsService.GetAllContests();

            IQueryable<Contest> contestsG;
            if (gym != null)
                contestsG = contests.Where(c => c.Gym == gym);
            else
                contestsG = contests;

            var res = contestsG;
            switch (sortByProp.ToLower())
            {
                case "contestid":
                    res = sortOrder == true ? contestsG.OrderBy(c => c.ContestId) : contestsG.OrderByDescending(c => c.ContestId);
                    break;
                case "name":
                    res = sortOrder == true ? contestsG.OrderBy(c => c.Name) : contestsG.OrderByDescending(c => c.Name);
                    break;
                case "starttime":
                    res = sortOrder == true ? contestsG.OrderBy(c => c.StartTime) : contestsG.OrderByDescending(c => c.StartTime);
                    break;
            }

            var contestsResponse = res.Skip((page - 1) * pageSize).Take(pageSize);

            ContestsWithPropsResponse response = new ContestsWithPropsResponse
            (
                Contests: contestsResponse.Select(c => new ContestsResponse(c.ContestId, c.Name, c.StartTime)),
                Properties: ["ContestId", "Name", "StartTime"]
            );

            return Ok(response);
        }
    }
}