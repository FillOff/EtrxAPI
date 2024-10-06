using Etrx.API.Contracts.Contests;
using Etrx.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Etrx.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContestsController : ControllerBase
    {
        private readonly IContestsService _contestsService;

        public ContestsController(IContestsService contestsService)
        {
            _contestsService = contestsService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContestsResponse>>> GetContests()
        {
            var contests = await _contestsService.GetAllContests();

            var response = contests.Select(c => new ContestsResponse(c.Id, c.Name));

            return Ok(response);
        }
    }
}
