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
        private readonly IExternalApiService _externalApiService;

        public CodeforcesController(
            IUsersService usersService,
            ICodeforcesService codeforcesService,
            IUpdateDataService updateDataService,
            IExternalApiService externalApiService)
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
        public async Task<IActionResult> PostSubmissionsFromCodeforces()
        {
            string[] handles = _usersService.GetUsersHandle();

            foreach (var handle in handles)
            {
                var (Submissions, Error) = await _externalApiService.GetCodeforcesSubmissionsAsync(handle);
                if (!string.IsNullOrEmpty(Error))
                    return StatusCode(StatusCodes.Status502BadGateway, Error);

                await _codeforcesService.PostSubmissionsFromCodeforces(Submissions!, handle);
            }

            return Ok("Submissions added successfully!");
        }

        [HttpPost("Submissions/PostSubmissionsFromCodeforcesByContestId")]
        public async Task<IActionResult> PostSubmissionsFromCodeforcesByContestId([FromQuery] int contestId)
        {
            var (handles, error) = await _externalApiService.GetCoodeforcesContestUsersAsync(_usersService.GetUsersHandle(), contestId);
            if (!string.IsNullOrEmpty(error))
                return StatusCode(StatusCodes.Status502BadGateway, error);

            Console.WriteLine(string.Join(';', handles));

            foreach (var handle in handles)
            {
                var (Submissions, Error) = await _externalApiService.GetCodeforcesContestSubmissionsAsync(handle, contestId);
                if (!string.IsNullOrEmpty(Error))
                    return StatusCode(StatusCodes.Status502BadGateway, Error);

                await _codeforcesService.PostSubmissionsFromCodeforces(Submissions!, handle);
            }

            return Ok($"Submissions of contest {contestId} added successfully!");
        }
    }
}