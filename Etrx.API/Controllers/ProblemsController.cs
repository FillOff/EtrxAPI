using AutoMapper;
using Etrx.API.Contracts.Contests;
using Etrx.API.Contracts.Problems;
using Etrx.Domain.Interfaces.Services;
using Etrx.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;

namespace Etrx.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProblemsController : ControllerBase
    {
        private readonly IProblemsService _problemsService;
        private readonly IMapper _mapper;

        public ProblemsController(IProblemsService problemsService, IMapper mapper)
        {
            _problemsService = problemsService;
            _mapper = mapper;
        }

        [HttpGet("GetProblemsByContestId")]
        public ActionResult<IEnumerable<ProblemsResponse>> GetProblemsByContestId([FromQuery] int contestId)
        {
            var problems = _problemsService.GetProblemsByContestId(contestId);

            var response = problems.Select(problem => _mapper.Map<ProblemsResponse>(problem)).AsEnumerable();

            return Ok(response);
        }

        [HttpGet("GetProblemsByPageWithSortAndFilterTags")]
        public ActionResult<ProblemsWithPropResponse> GetProblemsByPageWithSortAndFilterTags([FromQuery] int page, int pageSize, string? tags, string sortField = "id", bool sortOrder = true)
        {
            if (string.IsNullOrEmpty(sortField) || 
                !typeof(Problem).GetProperties().Any(p => p.Name.Equals(sortField, System.StringComparison.InvariantCultureIgnoreCase)))
            {
                return BadRequest($"Invalid field: {sortField}");
            }

            var result = _problemsService.GetProblemsByPageWithSortAndFilterTags(page, pageSize, tags, sortField, sortOrder);
            

            ProblemsWithPropResponse response = new ProblemsWithPropResponse
            (
                Problems: result.Problems.Select(problem => _mapper.Map<ProblemsResponse>(problem)).AsEnumerable(),
                Properties: typeof(ProblemsResponse).GetProperties().Select(p => p.Name).ToArray()!,
                pageCount: result.PageCount
            );

            return Ok(response);
        }

        [HttpGet("GetTagsList")]
        public ActionResult<List<string>> GetTagsList()
        {
            var response = _problemsService.GetAllTags();

            return Ok(response);
        }
    }
}
