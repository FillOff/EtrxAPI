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

        public CodeforcesController(IProblemsService problemsService)
        {
            _problemsService = problemsService;
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProblems()
        {
            HttpClient client = new HttpClient();

            var response = await client.GetAsync("https://codeforces.com/api/problemset.problems");

            var content = await response.Content.ReadAsStringAsync();

            var jsonDocument = JsonDocument.Parse(content);

            var problems = jsonDocument.RootElement.GetProperty("result").GetProperty("problems").EnumerateArray();

            foreach (var problem in problems)
            {
                int contestId = problem.GetProperty("contestId").GetInt32();
                string index = problem.GetProperty("index").GetString()!;
                string name = problem.GetProperty("name").GetString()!;
                string type = problem.GetProperty("type").GetString()!;
                double? points = problem.TryGetProperty("points", out var pointsProperty) ? pointsProperty.GetDouble() : (double?)null;
                int? rating = problem.TryGetProperty("rating", out var ratingProperty) ? ratingProperty.GetInt32() : (int?)null;
                string[] tags = problem.GetProperty("tags").EnumerateArray().Select(tag => tag.GetString()).ToArray()!;

                Problem newProblem = Domain.Models.Problem.Create(0, contestId, index, name, type, points, rating, tags);

                await _problemsService.CreateProblem(newProblem);
            }

            return Ok("Problems updated successfully");
        }
    }
}
