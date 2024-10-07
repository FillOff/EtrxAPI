using Azure;
using Etrx.Domain.Interfaces.Services;
using Etrx.Domain.Models;
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

        public CodeforcesController(IProblemsService problemsService, IContestsService contestsService, IUsersService usersService)
        {
            _problemsService = problemsService;
            _contestsService = contestsService;
            _usersService = usersService;
        }

        [HttpGet("Problems/GetProblemsFromCodeforces")]
        public async Task<IActionResult> GetProblemsFromCodeforces()
        {
            HttpClient client = new HttpClient();

            var response = await client.GetAsync("https://codeforces.com/api/problemset.problems");

            var content = await response.Content.ReadAsStringAsync();

            var jsonDocument = JsonDocument.Parse(content);

            var problems = jsonDocument.RootElement.GetProperty("result").GetProperty("problems").EnumerateArray();
            var statistics = jsonDocument.RootElement.GetProperty("result").GetProperty("problemStatistics").EnumerateArray();

            foreach (var problem in problems)
            {
                int contestId = problem.GetProperty("contestId").GetInt32();
                string index = problem.GetProperty("index").GetString()!;
                string name = problem.GetProperty("name").GetString()!;
                string type = problem.GetProperty("type").GetString()!;
                double? points = problem.TryGetProperty("points", out var pointsProperty) ? pointsProperty.GetDouble() : (double?)null;
                int? rating = problem.TryGetProperty("rating", out var ratingProperty) ? ratingProperty.GetInt32() : (int?)null;
                string[] tags = problem.GetProperty("tags").EnumerateArray().Select(tag => tag.GetString()).ToArray()!;

                int? solvedCount = statistics.First(stat =>
                    stat.GetProperty("contestId").GetInt32() == contestId &&
                    stat.GetProperty("index").GetString() == index)
                        .GetProperty("solvedCount").GetInt32();

                var newProblem = new Problem(0, contestId, index, name, type, points, rating, solvedCount, tags);

                await _problemsService.CreateProblem(newProblem);
            }

            return Ok("Problems added successfully");
        }

        [HttpGet("Contests/GetContestsFromCodeforces")]
        public async Task<IActionResult> GetContestsFromCodeforces()
        {
            using HttpClient client = new HttpClient();
            var response = await client.GetAsync("https://codeforces.com/api/contest.list?gym=false");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Не удалось получить данные с Codeforces.");
            }

            var content = await response.Content.ReadAsStringAsync();
            var contestsArray = JsonDocument.Parse(content).RootElement.GetProperty("result").EnumerateArray();

            foreach (var contestElement in contestsArray)
            {
                int contestId = contestElement.GetProperty("id").GetInt32();
                string name = contestElement.GetProperty("name").GetString()!;
                string type = contestElement.GetProperty("type").GetString()!;
                string phase = contestElement.GetProperty("phase").GetString()!;
                bool frozen = contestElement.GetProperty("frozen").GetBoolean();
                int durationSeconds = contestElement.GetProperty("durationSeconds").GetInt32();
                DateTime? startTimeSeconds = contestElement.TryGetProperty("startTimeSeconds", out var startTime) ? DateTimeOffset.FromUnixTimeSeconds(startTime.GetInt64()).UtcDateTime : null;
                int? relativeTimeSeconds = contestElement.TryGetProperty("relativeTimeSeconds", out var relativeTime) ? relativeTime.GetInt32() : (int?)null;
                string? preparedBy = contestElement.TryGetProperty("preparedBy", out var prepared) ? prepared.GetString() : null;
                string? websiteUrl = contestElement.TryGetProperty("websiteUrl", out var website) ? website.GetString() : null;
                string? description = contestElement.TryGetProperty("description", out var desc) ? desc.GetString() : null;
                int? difficulty = contestElement.TryGetProperty("difficulty", out var diff) ? diff.GetInt32() : (int?)null;
                string? kind = contestElement.TryGetProperty("kind", out var kindProp) ? kindProp.GetString() : null;
                string? icpcRegion = contestElement.TryGetProperty("icpcRegion", out var region) ? region.GetString() : null;
                string? country = contestElement.TryGetProperty("country", out var countryProp) ? countryProp.GetString() : null;
                string? city = contestElement.TryGetProperty("city", out var cityProp) ? cityProp.GetString() : null;
                string? season = contestElement.TryGetProperty("season", out var seasonProp) ? seasonProp.GetString() : null;

                var contest = new Contest(contestId, name, type, phase, frozen, durationSeconds, startTimeSeconds, relativeTimeSeconds, preparedBy, websiteUrl, description, difficulty, kind, icpcRegion, country, city, season, false);

                await _contestsService.CreateContest(contest);
            }

            response = await client.GetAsync("https://codeforces.com/api/contest.list?gym=true");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Не удалось получить данные с Codeforces.");
            }

            content = await response.Content.ReadAsStringAsync();
            contestsArray = JsonDocument.Parse(content).RootElement.GetProperty("result").EnumerateArray();

            foreach (var contestElement in contestsArray)
            {
                int contestId = contestElement.GetProperty("id").GetInt32();
                string name = contestElement.GetProperty("name").GetString()!;
                string type = contestElement.GetProperty("type").GetString()!;
                string phase = contestElement.GetProperty("phase").GetString()!;
                bool frozen = contestElement.GetProperty("frozen").GetBoolean();
                int durationSeconds = contestElement.GetProperty("durationSeconds").GetInt32();
                DateTime? startTimeSeconds = contestElement.TryGetProperty("startTimeSeconds", out var startTime) ? DateTimeOffset.FromUnixTimeSeconds(startTime.GetInt64()).UtcDateTime : null;
                int? relativeTimeSeconds = contestElement.TryGetProperty("relativeTimeSeconds", out var relativeTime) ? relativeTime.GetInt32() : (int?)null;
                string? preparedBy = contestElement.TryGetProperty("preparedBy", out var prepared) ? prepared.GetString() : null;
                string? websiteUrl = contestElement.TryGetProperty("websiteUrl", out var website) ? website.GetString() : null;
                string? description = contestElement.TryGetProperty("description", out var desc) ? desc.GetString() : null;
                int? difficulty = contestElement.TryGetProperty("difficulty", out var diff) ? diff.GetInt32() : (int?)null;
                string? kind = contestElement.TryGetProperty("kind", out var kindProp) ? kindProp.GetString() : null;
                string? icpcRegion = contestElement.TryGetProperty("icpcRegion", out var region) ? region.GetString() : null;
                string? country = contestElement.TryGetProperty("country", out var countryProp) ? countryProp.GetString() : null;
                string? city = contestElement.TryGetProperty("city", out var cityProp) ? cityProp.GetString() : null;
                string? season = contestElement.TryGetProperty("season", out var seasonProp) ? seasonProp.GetString() : null;

                var contest = new Contest(contestId, name, type, phase, frozen, durationSeconds, startTimeSeconds, relativeTimeSeconds, preparedBy, websiteUrl, description, difficulty, kind, icpcRegion, country, city, season, true);

                await _contestsService.CreateContest(contest);
            }

            return Ok("Contests added successfully");
        }

        [HttpGet("Users/GetUsersFromCodeforces")]
        public async Task<IActionResult> GetUsersFromCodeforces()
        {
            using HttpClient client = new HttpClient();
            var responseDl = await client.GetAsync("https://dl.gsu.by/codeforces/api/students");

            if (!responseDl.IsSuccessStatusCode)
            {
                throw new Exception("Не удалось получить данные пользователей с Dl.");
            }

            var usersDl = await responseDl.Content.ReadAsStringAsync();
            var usersDlArray = JsonDocument.Parse(usersDl).RootElement.EnumerateArray();

            foreach (var userDl in usersDlArray)
            {
                string handle = userDl.GetProperty("nick_name").ToString()!;
                string firstName = userDl.GetProperty("first_name").ToString()!;
                string lastName = userDl.GetProperty("last_name").ToString()!;
                int? grade = userDl.GetProperty("grade").GetInt32();

                var responseCodeforces = await client.GetAsync($"https://codeforces.com/api/user.info?handles={handle}");

                if (!responseCodeforces.IsSuccessStatusCode)
                {
                    //throw new Exception($"Не удалось получить данные пользователя {handle} с Codeforces.");
                    continue;
                }

                var usersCodeforces = await responseCodeforces.Content.ReadAsStringAsync();
                var usersCf = JsonDocument.Parse(usersCodeforces).RootElement.GetProperty("result").EnumerateArray();

                foreach (var userCf in usersCf)
                {
                    string? email = userCf.TryGetProperty("email", out var email1) ? email1.ToString() : null;
                    string? vkId = userCf.TryGetProperty("vkId", out var vkId1) ? vkId1.ToString() : null;
                    string? openId = userCf.TryGetProperty("openId", out var openId1) ? openId1.ToString() : null;
                    string? country = userCf.TryGetProperty("country", out var country1) ? country1.ToString() : null;
                    string? city = userCf.TryGetProperty("city", out var city1) ? city1.ToString() : null;
                    string? organization = userCf.TryGetProperty("organization", out var organization1) ? organization1.ToString() : null;
                    int? contribution = userCf.TryGetProperty("contribution", out var contribution1) ? contribution1.GetInt32() : null;
                    string? rank = userCf.TryGetProperty("rank", out var rank1) ? rank1.ToString() : null;
                    int? rating = userCf.TryGetProperty("rating", out var rating1) ? rating1.GetInt32() : null;
                    string? maxRank = userCf.TryGetProperty("maxRank", out var maxRank1) ? maxRank1.ToString() : null;
                    int? maxRating = userCf.TryGetProperty("maxRating", out var maxRating1) ? maxRating1.GetInt32() : null;
                    long? lastOnlineTimeSeconds = userCf.TryGetProperty("lastOnlineTimeSeconds", out var lastOnlineTimeSeconds1) ? lastOnlineTimeSeconds1.GetInt64() : null;
                    long? registrationTimeSeconds = userCf.TryGetProperty("registrationTimeSeconds", out var registrationTimeSeconds1) ? registrationTimeSeconds1.GetInt64() : null;
                    int? friendOfCount = userCf.TryGetProperty("friendOfCount", out var friendOfCount1) ? friendOfCount1.GetInt32() : null;
                    string? avatar = userCf.TryGetProperty("avatar", out var avatar1) ? avatar1.ToString() : null;
                    string? titlePhoto = userCf.TryGetProperty("titlePhoto", out var titlePhoto1) ? titlePhoto1.ToString() : null;

                    var user = new User(0, handle, null, null, null, firstName, lastName, country, city, organization, contribution, rank, rating, 
                                        maxRank, maxRating, lastOnlineTimeSeconds, registrationTimeSeconds, friendOfCount, avatar, titlePhoto, grade, null, true);

                    await _usersService.CreateUser(user);
                }
            }

            return Ok("Dl Users added successfully");
        }
    }
}
