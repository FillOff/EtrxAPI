using Etrx.API.Contracts.Contests;
using Etrx.Application.Services;
using Etrx.Domain.Interfaces.Services;
using Etrx.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;

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
        public ActionResult<ContestsWithPropsResponse> GetContestsByPage([FromQuery] int page, int pageSize, bool? gym, string sortField = "contestid", bool sortOrder = true)
        {
            var contests = gym != null 
                                ? _contestsService.GetAllContests().Where(c => c.Gym == gym) 
                                : _contestsService.GetAllContests();

            string order = sortOrder == true ? "asc" : "desc";
            string field = sortField.ToLower();

            if (string.IsNullOrEmpty(field) || !typeof(Problem).GetProperties().Any(p => p.Name.Equals(field, System.StringComparison.InvariantCultureIgnoreCase)))
            {
                return BadRequest($"Invalid field: {field}");
            }

            var sortedContests = contests
                                    .OrderBy($"{field} {order}")
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize);

            ContestsWithPropsResponse response = new ContestsWithPropsResponse
            (
                Contests: sortedContests.Select(c => new ContestsResponse(c.ContestId, c.Name, c.StartTime)),
                Properties: ["ContestId", "Name", "StartTime"]
            );

            return Ok(response);
        }

        [HttpGet("GetCountOfPagesContests")]
        public int GetCountOfPagesContests(int pageCount)
        {
            return _contestsService.GetAllContests().Count() / pageCount;
        }
    }
}