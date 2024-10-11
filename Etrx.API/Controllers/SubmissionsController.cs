using Etrx.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

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

        /*[HttpGet("GetSubmissionsByContestId")]
        public IActionResult GetSubmissionsByContestId(int contestId)
        {

        }*/
    }
}