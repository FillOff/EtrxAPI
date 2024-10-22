using AutoMapper.Configuration.Annotations;
using Etrx.Domain.Interfaces.Services;
using Etrx.Domain.Parsing_models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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
            var response = await _externalApiService.GetCodeforcesProblemsAsync();

            if (!string.IsNullOrEmpty(response.Error))
                return StatusCode(StatusCodes.Status502BadGateway, response.Error);

            await _codeforcesService.PostProblemsFromCodeforces(response.Problems!, response.ProblemStatistics!);

            return Ok("Problems added successfully");
        }

        [HttpPost("Contests/PostAndUpdateContestsFromCodeforces")]
        public async Task<IActionResult> PostAndUpdateContestsFromCodeforces()
        {
            var response = await _externalApiService.GetCodeforcesContestsAsync(false);

            if (!string.IsNullOrEmpty(response.Error))
                return StatusCode(StatusCodes.Status502BadGateway, response.Error);

            await _codeforcesService.PostContestsFromCodeforces(response.Contests!, false);

            response = await _externalApiService.GetCodeforcesContestsAsync(true);

            if (!string.IsNullOrEmpty(response.Error))
                return StatusCode(StatusCodes.Status502BadGateway, response.Error);

            await _codeforcesService.PostContestsFromCodeforces(response.Contests!, true);

            return Ok("Contests added successfully");
        }

        [HttpPost("Users/PostAndUpdateUsersFromDlCodeforces")]
        public async Task<IActionResult> PostAndUpdateUsersFromDlCodeforces()
        {
            var dlUsers = await _externalApiService.GetDlUsersAsync();

            if (!string.IsNullOrEmpty(dlUsers.Error))
                return StatusCode(StatusCodes.Status502BadGateway, dlUsers.Error);

            string handlesString = string.Join(';', dlUsers.Users!.Select(user => user.Handle.ToLower()));
            var CodeforcesUsers = await _externalApiService.GetCodeforcesUsersAsync(handlesString);

            if (!string.IsNullOrEmpty(CodeforcesUsers.Error))
                return StatusCode(StatusCodes.Status502BadGateway, CodeforcesUsers.Error);

            await _codeforcesService.PostUsersFromDlCodeforces(dlUsers.Users!, CodeforcesUsers.Users!);

            return Ok("Dl users added successfully");
        }

        [HttpPost("Submissions/PostAndUpdateSubmissionsFromCodeforces")]
        public async Task<IActionResult> PostSubmissionsFromCodeforces()
        {
            string[] handles = _usersService.GetAllUsers().Select(u => u.Handle).ToArray();

            foreach (var handle in handles)
            {
                var response = await _externalApiService.GetCodeforcesSubmissionsAsync(handle);
                if (!string.IsNullOrEmpty(response.Error))
                    return StatusCode(StatusCodes.Status502BadGateway, response.Error);

                await _codeforcesService.PostSubmissionsFromCodeforces(response.Submissions!, handle);
            }

            return Ok("Submissions added successfully!");
        }

        [HttpPost("PostSubmissionsFromCodeforcesByContestId")]
        public async Task<IActionResult> PostSubmissionsFromCodeforcesByContestId(int contestId)
        {
            string[] handles = _usersService.GetAllUsers().Select(u => u.Handle).ToArray();

            foreach (var handle in handles)
            {
                var response = await _externalApiService.GetCodeforcesContestSubmissionsAsync(handle, contestId);
                if (!string.IsNullOrEmpty(response.Error))
                    return StatusCode(StatusCodes.Status502BadGateway, response.Error);

                await _codeforcesService.PostSubmissionsFromCodeforces(response.Submissions!, handle);
            }

            return Ok($"Submissions of contest {contestId} added successfully!");
        }
    }
}