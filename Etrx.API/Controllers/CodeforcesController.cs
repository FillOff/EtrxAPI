using Etrx.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Etrx.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CodeforcesController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly ICodeforcesService _codeforcesService;
        private readonly IExternalApiService _externalApiService;

        public CodeforcesController(IUsersService usersService,
                                    ICodeforcesService codeforcesService,
                                    IExternalApiService externalApiService)
        {
            _usersService = usersService;
            _codeforcesService = codeforcesService;
            _externalApiService = externalApiService;
        }

        [HttpPost("Problems/PostAndUpdateProblemsFromCodeforces")]
        public async Task<IActionResult> PostAndUpdateProblemsFromCodeforces()
        {
            var (Problems, ProblemStatistics, Error) = await _externalApiService.GetCodeforcesProblemsAsync();

            if (!string.IsNullOrEmpty(Error))
                return StatusCode(StatusCodes.Status502BadGateway, Error);

            await _codeforcesService.PostProblemsFromCodeforces(Problems!, ProblemStatistics!);

            return Ok("Problems added successfully");
        }

        [HttpPost("Contests/PostAndUpdateContestsFromCodeforces")]
        public async Task<IActionResult> PostAndUpdateContestsFromCodeforces()
        {
            var (Contests, Error) = await _externalApiService.GetCodeforcesContestsAsync(false);

            if (!string.IsNullOrEmpty(Error))
                return StatusCode(StatusCodes.Status502BadGateway, Error);

            await _codeforcesService.PostContestsFromCodeforces(Contests!, false);

            (Contests, Error) = await _externalApiService.GetCodeforcesContestsAsync(true);

            if (!string.IsNullOrEmpty(Error))
                return StatusCode(StatusCodes.Status502BadGateway, Error);

            await _codeforcesService.PostContestsFromCodeforces(Contests!, true);

            return Ok("Contests added successfully");
        }

        [HttpPost("Users/PostAndUpdateUsersFromDlCodeforces")]
        public async Task<IActionResult> PostAndUpdateUsersFromDlCodeforces()
        {
            var dlUsers = await _externalApiService.GetDlUsersAsync();

            if (!string.IsNullOrEmpty(dlUsers.Error))
                return StatusCode(StatusCodes.Status502BadGateway, dlUsers.Error);

            string handlesString = string.Join(';', dlUsers.Users!.Select(user => user.Handle.ToLower()));
            var (Users, Error) = await _externalApiService.GetCodeforcesUsersAsync(handlesString);

            if (!string.IsNullOrEmpty(Error))
                return StatusCode(StatusCodes.Status502BadGateway, Error);

            await _codeforcesService.PostUsersFromDlCodeforces(dlUsers.Users!, Users!);

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