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
        public async Task<ActionResult<IEnumerable<ProblemsResponse>>> GetProblemsByContestId(int id)
        {
            var problems = await _problemsService.GetProblemsByContestIdAsync(id);

            var response = problems.Select(problem => _mapper.Map<ProblemsResponse>(problem)).AsEnumerable();
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<ProblemsWithPropsResponse>> GetProblemsByPageWithSortAndFilterTags(
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery] string? tags,
            [FromQuery] string? indexes,
            [FromQuery] string? problemName,
            [FromQuery] int minRating = 0,
            [FromQuery] int maxRating = int.MaxValue,
            [FromQuery] double minPoints = 0,
            [FromQuery] double maxPoints = int.MaxValue,
            [FromQuery] string sortField = "id", 
            [FromQuery] bool sortOrder = true)
        {
            if (string.IsNullOrEmpty(sortField) || 
                !typeof(Problem).GetProperties().Any(p => p.Name.Equals(sortField, System.StringComparison.InvariantCultureIgnoreCase)))
            {
                return BadRequest($"Invalid field: {sortField}");
            }

            var (problems, pageCount) = await _problemsService.GetProblemsByPageWithSortAndFilterTagsAsync(
                page, 
                pageSize, 
                tags, 
                indexes, 
                problemName, 
                sortField, 
                sortOrder,
                minRating,
                maxRating,
                minPoints,
                maxPoints);
            
            ProblemsWithPropsResponse response = new ProblemsWithPropsResponse
            (
                Problems: _mapper.Map<List<ProblemsResponse>>(problems),
                Properties: typeof(ProblemsResponse).GetProperties().Select(p => p.Name).ToArray()!,
                PageCount: pageCount
            );

            return Ok(response);
        }

        [HttpGet("tags")]
        public async Task<ActionResult<List<string>>> GetTagsList()
        {
            var response = await _problemsService.GetAllTagsAsync();

            return Ok(response);
        }

        [HttpGet("indexes")]
        public async Task<ActionResult<List<string>>> GetIndexesList()
        {
            var response = await _problemsService.GetAllIndexesAsync();

            return Ok(response);
        }
    }
}
