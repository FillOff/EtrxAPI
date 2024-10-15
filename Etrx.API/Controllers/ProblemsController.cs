using AutoMapper;
using Etrx.API.Contracts.Problems;
using Etrx.Domain.Interfaces.Services;
using Etrx.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var problems = _problemsService.GetAllProblems();

            string order = sortOrder == true ? "asc" : "desc";
            string field = sortField.ToLower();

            if (string.IsNullOrEmpty(field) || !typeof(Problem).GetProperties().Any(p => p.Name.Equals(field, System.StringComparison.InvariantCultureIgnoreCase)))
            {
                return BadRequest($"Invalid field: {field}");
            }

            var sortedProblems = problems.OrderBy($"{field} {order}");

            var problemsFilter = sortedProblems.AsQueryable();
            if (tags != null)
            {
                var tagsFilter = tags.Split(';');
                problemsFilter = sortedProblems.AsQueryable().Where(p => tagsFilter.All(tag => p.Tags.Contains(tag)));
            }

            var problemsSort = problemsFilter.Skip((page - 1) * pageSize).Take(pageSize);

            ProblemsWithPropResponse response = new ProblemsWithPropResponse
            (
                Problems: problemsSort.Select(problem => _mapper.Map<ProblemsResponse>(problem)).AsEnumerable(),
                Properties: ["Id", "ContestId", "Index", "Name", "Points", "Rating", "Tags"]
            );

            return Ok(response);
        }

        [HttpGet("GetCountOfPagesProblems")]
        public int GetCountOfPagesProblems(int pageCount)
        {
            return _problemsService.GetAllProblems().Count() / pageCount;
        }

        [HttpGet("GetTagsList")]
        public async  Task<ActionResult<List<string>>> GetTagsList()
        {
            var problems = await _problemsService.GetAllProblems().ToListAsync();

            var tags = problems
                .SelectMany(problem => problem.Tags)
                .Distinct()
                .ToList();

            return Ok(tags);
        }
    }
}
