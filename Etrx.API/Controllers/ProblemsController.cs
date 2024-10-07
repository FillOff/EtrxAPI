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

        /*[HttpGet("GetAllProblems")]
        public ActionResult<IEnumerable<ProblemsResponse>> GetAllProblems()
        {
            var problems = _problemsService.GetAllProblems();

            var response = problems.Select(p => new ProblemsResponse(p.Id, p.ContestId, p.Index, p.Name, p.Points, p.Rating, p.Tags));

            return Ok(response);
        }*/

        [HttpGet("GetProblemsByContestId")]
        public ActionResult<IEnumerable<ProblemsResponse>> GetProblemsByContestId([FromQuery] int contestId)
        {
            var problems = _problemsService.GetProblemsByContestId(contestId);

            var response = problems.Select(p => new ProblemsResponse(p.Id, p.ContestId, p.Index, p.Name, p.Points, p.Rating, p.Tags));

            return Ok(response);
        }

        [HttpGet("GetProblemsByPage")]
        public ActionResult<IEnumerable<ProblemsResponse>> GetProblemsByPage([FromQuery] int page, int pageSize)
        {
            var problems = _problemsService.GetAllProblems()
                .Skip((page-1) * pageSize)
                .Take(pageSize);

            var problemsResponse = problems.Select(p => new ProblemsResponse(p.Id, p.ContestId, p.Index, p.Name, p.Points, p.Rating, p.Tags));

            return Ok(problemsResponse);
        }
    }
}
