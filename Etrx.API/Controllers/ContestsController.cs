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

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ContestsResponse>> GetContestById(int id)
        {
            var contest = await _contestsService.GetContestByIdAsync(id);

            if (contest == null)
                return NotFound($"Contest {id} not fount");

            var response = _mapper.Map<ContestsResponse>(contest);
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<ContestsWithPropsResponse>> GetContestsByPageWithSort(
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

            var (contests, pageCount) = await _contestsService.GetContestsByPageWithSortAsync(
                page, 
                pageSize, 
                gym, 
                sortField, 
                sortOrder);

            var response = new ContestsWithPropsResponse
            (
                Contests: _mapper.Map<List<ContestsResponse>>(contests),
                Properties: typeof(ContestsResponse).GetProperties().Select(p => p.Name).ToArray(),
                PageCount: pageCount
            );

            return Ok(response);
        }
    }
}