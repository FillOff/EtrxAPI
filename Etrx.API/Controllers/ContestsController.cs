using AutoMapper;
using Etrx.API.Contracts.Contests;
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

        public ContestsController(IContestsService contestsService, 
                                  IMapper mapper)
        {
            _contestsService = contestsService;
            _mapper = mapper;
        }

        [HttpGet("GetContestById")]
        public ActionResult<ContestsResponse> GetContestById([FromQuery] int contestId)
        {
            var contest = _contestsService.GetContestById(contestId);

            if (contest == null)
                return NotFound($"Contest {contestId} not fount");

            var response = _mapper.Map<ContestsResponse>(contest);
            return Ok(response);
        }

        [HttpGet("GetContestsByPageWithSort")]
        public ActionResult<ContestsWithPropsResponse> GetContestsByPageWithSort(
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery] bool? gym,
            [FromQuery] string sortField = "contestid",
            [FromQuery] bool sortOrder = true)
        {
            if (string.IsNullOrEmpty(sortField) || 
                !typeof(Contest).GetProperties().Any(p => p.Name.Equals(sortField, System.StringComparison.InvariantCultureIgnoreCase)))
            {
                return BadRequest($"Invalid field: {sortField}");
            }

            var (Contests, PageCount) = _contestsService.GetContestsByPageWithSort(page, pageSize, gym, sortField, sortOrder);

            var response = new ContestsWithPropsResponse
            (
                Contests: Contests.Select(contest => _mapper.Map<ContestsResponse>(contest)).AsEnumerable(),
                Properties: typeof(ContestsResponse).GetProperties().Select(p => p.Name).ToArray(),
                PageCount: PageCount
            );

            return Ok(response);
        }
    }
}