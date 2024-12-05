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
        private readonly ICodeforcesApiService _externalApiService;

        public CodeforcesController(
            IUsersService usersService,
            ICodeforcesService codeforcesService,
            IUpdateDataService updateDataService,
            ICodeforcesApiService externalApiService)
        {
            _usersService = usersService;
            _codeforcesService = codeforcesService;
            _updateDataService = updateDataService;
            _externalApiService = externalApiService;
        }

        [HttpPost("Problems/PostAndUpdateProblemsFromCodeforces")]
        public async Task<IActionResult> PostAndUpdateProblemsFromCodeforces()
        {
            var (_, Error) = await _updateDataService.UpdateProblems();

            if (!string.IsNullOrEmpty(Error))
                return StatusCode(StatusCodes.Status502BadGateway, Error);

            return Ok("Problems added successfully");
        }

        [HttpPost("Contests/PostAndUpdateContestsFromCodeforces")]
        public async Task<IActionResult> PostAndUpdateContestsFromCodeforces()
        {
            var (_, Error) = await _updateDataService.UpdateContests();

            if (!string.IsNullOrEmpty(Error))
                return StatusCode(StatusCodes.Status502BadGateway, Error);

            return Ok("Contests added successfully");
        }

        [HttpPost("Users/PostAndUpdateUsersFromDlCodeforces")]
        public async Task<IActionResult> PostAndUpdateUsersFromDlCodeforces()
        {
            var (_, Error) = await _updateDataService.UpdateUsers();

            if (!string.IsNullOrEmpty(Error))
                return StatusCode(StatusCodes.Status502BadGateway, Error);

            return Ok("Dl users added successfully");
        }

        [HttpPost("Submissions/PostAndUpdateSubmissionsFromCodeforces")]
        public async Task<IActionResult> PostAndUpdateSubmissionsFromCodeforces()
        {
            var (_, Error) = await _updateDataService.UpdateSubmissions();

            if (!string.IsNullOrEmpty(Error))
                return StatusCode(StatusCodes.Status502BadGateway, Error);

            return Ok("Submissions added successfully!");
        }

        [HttpPost("Submissions/PostAndUpdateSubmissionsFromCodeforcesByContestId")]
        public async Task<IActionResult> PostAndUpdateSubmissionsFromCodeforcesByContestId([FromQuery] int contestId)
        {
            var handles = await _externalApiService.GetCodeforcesContestUsersAsync(_usersService.GetHandles(), contestId);

            Console.WriteLine(string.Join(';', handles));

            foreach (var handle in handles)
            {
                var submissions = await _externalApiService.GetCodeforcesContestSubmissionsAsync(handle, contestId);

                await _codeforcesService.PostSubmissionsFromCodeforces(submissions, handle);
            }

            return Ok($"Submissions of contest {contestId} added successfully!");
        }
    }
}