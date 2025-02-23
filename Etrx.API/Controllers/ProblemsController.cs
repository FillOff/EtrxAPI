using AutoMapper;
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

        public ProblemsController(
            IProblemsService problemsService, 
            IMapper mapper)
        {
            _problemsService = problemsService;
            _mapper = mapper;
        }

        [HttpGet("{id:int}")]
        public ActionResult<IEnumerable<ProblemsResponse>> GetProblemsByContestId(int id)
        {
            var problems = _problemsService.GetProblemsByContestId(id);

            var response = problems.Select(problem => _mapper.Map<ProblemsResponse>(problem)).AsEnumerable();
            return Ok(response);
        }

        [HttpGet]
        public ActionResult<ProblemsWithPropsResponse> GetProblemsByPageWithSortAndFilterTags(
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery] string? tags,
            [FromQuery] string? indexes,
            [FromQuery] string? problemName,
            [FromQuery] string sortField = "id", 
            [FromQuery] bool sortOrder = true)
        {
            if (string.IsNullOrEmpty(sortField) || 
                !typeof(Problem).GetProperties().Any(p => p.Name.Equals(sortField, System.StringComparison.InvariantCultureIgnoreCase)))
            {
                return BadRequest($"Invalid field: {sortField}");
            }

            var (Problems, PageCount) = _problemsService.GetProblemsByPageWithSortAndFilterTags(page, pageSize, tags, indexes, problemName, sortField, sortOrder);
            
            ProblemsWithPropsResponse response = new ProblemsWithPropsResponse
            (
                Problems: Problems.Select(problem => _mapper.Map<ProblemsResponse>(problem)).AsEnumerable(),
                Properties: typeof(ProblemsResponse).GetProperties().Select(p => p.Name).ToArray()!,
                PageCount: PageCount
            );

            return Ok(response);
        }

        [HttpGet("tags")]
        public ActionResult<List<string>> GetTagsList()
        {
            var response = _problemsService.GetAllTags();

            return Ok(response);
        }

        [HttpGet("indexes")]
        public ActionResult<List<string>> GetIndexesList()
        {
            var response = _problemsService.GetAllIndexes();

            return Ok(response);
        }
    }
}
