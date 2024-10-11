using Etrx.API.Contracts.Problems;
using Etrx.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Etrx.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProblemsController : ControllerBase
    {
        private readonly IProblemsService _problemsService;

        public ProblemsController(IProblemsService problemsService)
        {
            _problemsService = problemsService;
        }

        [HttpGet("GetAllProblems")]
        public ActionResult<IEnumerable<ProblemsResponse>> GetAllProblems()
        {
            var problems = _problemsService.GetAllProblems();

            var response = problems.Select(p => new ProblemsResponse(p.Id, p.ContestId, p.Index, p.Name, p.Points, p.Rating, p.Tags)).AsEnumerable();

            return Ok(response);
        }

        [HttpGet("GetProblemsByContestId")]
        public ActionResult<IEnumerable<ProblemsResponse>> GetProblemsByContestId([FromQuery] int contestId)
        {
            var problems = _problemsService.GetProblemsByContestId(contestId);

            var response = problems.Select(p => new ProblemsResponse(p.Id, p.ContestId, p.Index, p.Name, p.Points, p.Rating, p.Tags)).AsEnumerable();

            return Ok(response);
        }

        [HttpGet("GetProblemsByPageWithSortAndFilterTags")]
        public ActionResult<ProblemsWithPropResponse> GetProblemsByPageWithSortAndFilterTags([FromQuery] int page, int pageSize, string? sortByProp, bool? sortOrder, string? tags)
        {
            var problems = _problemsService.GetAllProblems();

            sortOrder ??= true;
            sortByProp ??= "id";


            var res = problems;
            switch (sortByProp.ToLower())
            {
                case "id":
                    res = sortOrder == true ? problems.OrderBy(p => p.Id) : problems.OrderByDescending(p => p.Id);
                    break;
                case "contestid":
                    res = sortOrder == true ? problems.OrderBy(p => p.ContestId) : problems.OrderByDescending(p => p.ContestId);
                    break;
                case "index":
                    res = sortOrder == true ? problems.OrderBy(p => p.Index) : problems.OrderByDescending(p => p.Index);
                    break;
                case "name":
                    res = sortOrder == true ? problems.OrderBy(p => p.Name) : problems.OrderByDescending(p => p.Name);
                    break;
                case "points":
                    res = sortOrder == true ? problems.OrderBy(p => p.Points) : problems.OrderByDescending(p => p.Points);
                    break;
                case "rating":
                    res = sortOrder == true ? problems.OrderBy(p => p.Rating) : problems.OrderByDescending(p => p.Rating);
                    break;
                case "tags":
                    res = sortOrder == true ? problems.OrderBy(p => p.Tags) : problems.OrderByDescending(p => p.Tags);
                    break;
            }

            var problemsFilter = res;
            if (tags != null)
            {
                var tagsFilter = tags.Split(';');
                problemsFilter = res.Where(p => tagsFilter.All(tag => p.Tags.Contains(tag)));
            }

            var problemsSort = problemsFilter.Skip((page - 1) * pageSize).Take(pageSize);


            ProblemsWithPropResponse response = new ProblemsWithPropResponse
            (
                Problems: problemsSort.Select(p => new ProblemsResponse(p.Id, p.ContestId, p.Index, p.Name, p.Points, p.Rating, p.Tags)).AsEnumerable(),
                Properties: ["Id", "ContestId", "Index", "Name", "Points", "Rating", "Tags"]
            );

            return Ok(response);
        }
    }
}
