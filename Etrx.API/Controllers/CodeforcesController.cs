using Etrx.Domain.Interfaces.Services;
using Etrx.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Etrx.API.Controllers
{
    [ApiController]
    [Route("")]
    public class CodeforcesController : ControllerBase
    {
        private readonly IProblemsService _problemsService;
        private readonly IContestsService _contestsService;

        public CodeforcesController(IProblemsService problemsService, IContestsService contestsService)
        {
            _problemsService = problemsService;
            _contestsService = contestsService;
        }

        [HttpGet("api/[controller]/Problems")]
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

            return Ok("Problems updated successfully");
        }

        [HttpGet("api/[controller]/Contests")]
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
                int id = contestElement.GetProperty("id").GetInt32();
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

                var contest = new Contest(0, id, name, type, phase, frozen, durationSeconds, startTimeSeconds, relativeTimeSeconds, preparedBy, websiteUrl, description, difficulty, kind, icpcRegion, country, city, season, false);

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
                int id = contestElement.GetProperty("id").GetInt32();
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

                var contest = new Contest(0,id, name, type, phase, frozen, durationSeconds, startTimeSeconds, relativeTimeSeconds, preparedBy, websiteUrl, description, difficulty, kind, icpcRegion, country, city, season, true);

                await _contestsService.CreateContest(contest);
            }

            return Ok("Contests updated successfully");
        }
    }
}
