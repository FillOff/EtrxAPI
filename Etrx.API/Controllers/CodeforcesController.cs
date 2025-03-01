using Etrx.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Etrx.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CodeforcesController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly ICodeforcesService _codeforcesService;
        private readonly IUpdateDataService _updateDataService;
        private readonly ICodeforcesApiService _codeforcesApiService;

        public CodeforcesController(
            IUsersService usersService,
            ICodeforcesService codeforcesService,
            IUpdateDataService updateDataService,
            ICodeforcesApiService externalApiService)
        {
            _usersService = usersService;
            _codeforcesService = codeforcesService;
            _updateDataService = updateDataService;
            _codeforcesApiService = externalApiService;
        }

        [HttpPost("problems")]
        public async Task<IActionResult> PostAndUpdateProblemsFromCodeforces()
        {
            var (_, Error) = await _updateDataService.UpdateProblems();

            if (!string.IsNullOrEmpty(Error))
                return StatusCode(StatusCodes.Status502BadGateway, Error);

            return Ok("Problems added successfully");
        }

        [HttpPost("contests")]
        public async Task<IActionResult> PostAndUpdateContestsFromCodeforces()
        {
            var (_, Error) = await _updateDataService.UpdateContests();

            if (!string.IsNullOrEmpty(Error))
                return StatusCode(StatusCodes.Status502BadGateway, Error);

            return Ok("Contests added successfully");
        }

        [HttpPost("users")]
        public async Task<IActionResult> PostAndUpdateUsersFromDlCodeforces()
        {
            var (_, Error) = await _updateDataService.UpdateUsers();

            if (!string.IsNullOrEmpty(Error))
                return StatusCode(StatusCodes.Status502BadGateway, Error);

            return Ok("Dl users added successfully");
        }

        [HttpPost("submissions/{contestId:int}")]
        public async Task<IActionResult> PostAndUpdateSubmissionsFromCodeforcesByContestId(int contestId)
        {
            var handles = await _codeforcesApiService.GetCodeforcesContestUsersAsync(await _usersService.GetHandlesAsync(), contestId);

            foreach (var handle in handles)
            {
                var submissions = await _codeforcesApiService.GetCodeforcesContestSubmissionsAsync(handle, contestId);

                await _codeforcesService.PostSubmissionsFromCodeforces(submissions, handle);
            }

            return Ok($"Submissions of contest {contestId} added successfully!");
        }
    }
}