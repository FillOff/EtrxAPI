using Etrx.Core.Models;
using Etrx.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Etrx.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubmissionsController : ControllerBase
    {
        private readonly ISubmissionsService _submissionsService;

        public SubmissionsController(ISubmissionsService submissionsService)
        {
            _submissionsService = submissionsService;
        }

        [HttpGet("GetSubmissionsByContestId")]
        public async Task<ActionResult<Submission>> GetSubmissionsByContestId(int contestId)
        {
            var submissions = await _submissionsService.GetAllSubmissions().Where(s => s.ContestId == contestId).ToArrayAsync();

            return Ok(submissions);
        }
    }
}