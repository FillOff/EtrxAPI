using AutoMapper;
using Etrx.API.Contracts.Contests;
using Etrx.API.Contracts.Users;
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
        private readonly IMapper _mapper;

        public ContestsController(IContestsService contestsService, IMapper mapper)
        {
            _contestsService = contestsService;
            _mapper = mapper;
        }

        [HttpGet("GetContestById")]
        public ActionResult<ContestsResponse> GetContestById([FromQuery] int contestId)
        {
            var contest = _contestsService.GetContestById(contestId);

            if (contest != null)
            {
                var response = _mapper.Map<ContestsResponse>(contest);
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
                Contests: sortedContests.Select(contest => _mapper.Map<ContestsResponse>(contest)).AsEnumerable(),
                Properties: ["ContestId", "Name", "StartTime"]
            );

            return Ok(response);
        }

        [HttpGet("GetCountOfPagesContests")]
        public int GetCountOfPagesContests(int pageCount)
        {
            return _contestsService.GetAllContests().Count() / pageCount + 1;
        }
    }
}