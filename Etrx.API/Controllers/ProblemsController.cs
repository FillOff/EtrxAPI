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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProblemsResponse>>> GetProblems()
        {
            var problems = await _problemsService.GetAllProblems();

            var response = problems.Select(p => new ProblemsResponse(p.Id, p.ContestId, p.Index, p.Name));

            return Ok(response);
        }
    }
}
