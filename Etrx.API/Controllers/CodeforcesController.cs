using Etrx.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Etrx.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CodeforcesController : ControllerBase
    {
        private readonly IProblemsService _problemsService;
        private readonly IContestsService _contestsService;
        private readonly IUsersService _usersService;
        private readonly IJsonService _jsonService;
        private readonly ISubmissionsService _submissionsService;

        public CodeforcesController(IProblemsService problemsService, 
                                    IContestsService contestsService, 
                                    IUsersService usersService, 
                                    IJsonService jsonService,
                                    ISubmissionsService submissionsService)
        {
            _problemsService = problemsService;
            _contestsService = contestsService;
            _usersService = usersService;
            _jsonService = jsonService;
            _submissionsService = submissionsService;
        }

        [HttpPost("Problems/PostProblemsFromCodeforces")]
        public async Task<IActionResult> PostProblemsFromCodeforces()
        {
            using HttpClient client = new HttpClient();

            var response = await client.GetAsync("https://codeforces.com/api/problemset.problems");

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(StatusCodes.Status502BadGateway, "Couldn't get data from Codeforces.");
            }

            var content = await response.Content.ReadAsStringAsync();

            if (content[0] == '<')
            {
                return StatusCode(StatusCodes.Status502BadGateway, "Couldn't get data from Codeforces.");
            }

            var jsonDocument = JsonDocument.Parse(content);

            var problems = jsonDocument.RootElement.GetProperty("result").GetProperty("problems").EnumerateArray();
            var statistics = jsonDocument.RootElement.GetProperty("result").GetProperty("problemStatistics").EnumerateArray();

            foreach (var problem in problems)
            {
                var newProblem = _jsonService.JsonToProblem(problem, statistics);
                await _problemsService.CreateProblem(newProblem);
            }

            return Ok("Problems added successfully");
        }

        [HttpPost("Contests/PostContestsFromCodeforces")]
        public async Task<IActionResult> PostContestsFromCodeforces()
        {
            using HttpClient client = new HttpClient();
            var response = await client.GetAsync("https://codeforces.com/api/contest.list?gym=false");

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(StatusCodes.Status502BadGateway, "Couldn't get data from Codeforces.");
            }

            var content = await response.Content.ReadAsStringAsync();

            if (content[0] == '<')
            {
                return StatusCode(StatusCodes.Status502BadGateway, "Couldn't get data from Codeforces.");
            }

            var contestsArray = JsonDocument.Parse(content).RootElement.GetProperty("result").EnumerateArray();

            foreach (var contest in contestsArray)
            {
                var newContest = _jsonService.JsonToContest(contest, false);
                await _contestsService.CreateContest(newContest);
            }

            response = await client.GetAsync("https://codeforces.com/api/contest.list?gym=true");

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(StatusCodes.Status502BadGateway, "Couldn't get data from Codeforces.");
            }

            content = await response.Content.ReadAsStringAsync();
            contestsArray = JsonDocument.Parse(content).RootElement.GetProperty("result").EnumerateArray();

            foreach (var contest in contestsArray)
            {
                var newContest = _jsonService.JsonToContest(contest, true);
                await _contestsService.CreateContest(newContest);
            }

            return Ok("Contests added successfully");
        }

        [HttpPost("Users/PostUsersFromDlCodeforces")]
        public async Task<IActionResult> PostUsersFromDlCodeforces()
        {
            using HttpClient client = new HttpClient();
            var responseDl = await client.GetAsync("https://dl.gsu.by/codeforces/api/students");

            if (!responseDl.IsSuccessStatusCode)
            {
                return StatusCode(StatusCodes.Status502BadGateway, "Couldn't get data from Dl.");
            }

            var usersDl = await responseDl.Content.ReadAsStringAsync();
            var usersDlArray = JsonDocument.Parse(usersDl).RootElement.EnumerateArray();

            string handlesString = String.Join(';', usersDlArray.Select(userDl => userDl.GetProperty("nick_name").ToString().ToLower()));

            var handles = usersDlArray.Select(u => u.GetProperty("nick_name").ToString().ToLower()).ToArray();
            var firstNames = usersDlArray.Select(u => u.TryGetProperty("first_name", out var firstName1) ? firstName1.ToString() : null).ToArray();
            var lastNames = usersDlArray.Select(u => u.TryGetProperty("last_name", out var lastName1) ? lastName1.ToString() : null).ToArray();
            var grades = usersDlArray.Select(u => u.TryGetProperty("grade", out var grade1) ? grade1.GetInt32() : (int?)null).ToArray();

            var responseCodeforces = await client.GetAsync($"https://codeforces.com/api/user.info?handles={handlesString}&lang=ru");

            if (!responseCodeforces.IsSuccessStatusCode)
            {
                return StatusCode(StatusCodes.Status502BadGateway, "Couldn't get data from Codeforces.");
            }

            var usersCodeforces = await responseCodeforces.Content.ReadAsStringAsync();

            if (usersCodeforces[0] == '<')
            {
                return StatusCode(StatusCodes.Status502BadGateway, "Couldn't get data from Codeforces.");
            }

            var usersCodeforcesArray = JsonDocument.Parse(usersCodeforces).RootElement.GetProperty("result").EnumerateArray();

            for (int i = 0; i < usersCodeforcesArray.Count(); i++)
            {
                var newUser = _jsonService.JsonToUser(handles[i], firstNames[i], lastNames[i], grades[i],
                                                      usersCodeforcesArray.First(u => u.GetProperty("handle").ToString().ToLower() == handles[i]));

                await _usersService.CreateUser(newUser);
            }

            return Ok("Dl users added successfully");
        }

        [HttpPost("PostSubmissionsFromCodeforces")]
        public async Task<IActionResult> PostSubmissionsFromCodeforces()
        {
            string[] handles = _usersService.GetAllUsers().Select(u => u.Handle).ToArray();

            using HttpClient client = new HttpClient();

            foreach (var handle in handles)
            {
                var response = await client.GetAsync($"https://codeforces.com/api/user.status?handle={handle}");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(StatusCodes.Status502BadGateway, "Couldn't get data from Codeforces.");
                }

                var submissions = await response.Content.ReadAsStringAsync();

                if (submissions[0] == '<')
                {
                    return StatusCode(StatusCodes.Status502BadGateway, "Couldn't get data from Codeforces.");
                }

                var submissionsArray = JsonDocument.Parse(submissions).RootElement.GetProperty("result").EnumerateArray();

                foreach (var submissioncf in submissionsArray)
                {
                    var submission = _jsonService.JsonToSubmission(submissioncf);
                    await _submissionsService.CreateSubmission(submission);
                }
            }

            return Ok("Submissions added successfully!");
        }

        /*[HttpPost("PostSubmissionsFromCodeforcesByContestId")]
        public async Task<IActionResult> PostSubmissionsFromCodeforcesByContestId(int contestId)
        {
            string[] handles = _submissionsService.GetAllSubmissions().Where(s => s.ContestId == contestId).Select(s => s.Handle).Distinct().ToArray();

            using HttpClient client = new HttpClient();

            foreach (var handle in handles)
            {
                var response = await client.GetAsync($"https://codeforces.com/api/user.status?handle={handle}");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(StatusCodes.Status502BadGateway, "Couldn't get data from Codeforces.");
                }

                var submissions = await response.Content.ReadAsStringAsync();

                if (submissions[0] == '<')
                {
                    return StatusCode(StatusCodes.Status502BadGateway, "Couldn't get data from Codeforces.");
                }

                var submissionsArray = JsonDocument.Parse(submissions).RootElement.GetProperty("result").EnumerateArray();

                foreach (var submissioncf in submissionsArray)
                {
                    var submission = _jsonService.JsonToSubmission(submissioncf);
                    await _submissionsService.CreateSubmission(submission);
                }
            }

            return Ok("Submissions added successfully!");
        }*/
    }
}