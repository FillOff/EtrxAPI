using Etrx.Domain.Parsing_models;
using Etrx.Domain.Interfaces.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Etrx.Application.Services
{
    public class CodeforcesApiService(HttpClient httpClient) : ICodeforcesApiService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<List<DlUser>> GetDlUsersAsync()
            => await GetApiDataAsync<List<DlUser>>("https://dl.gsu.by/codeforces/api/students");

        public async Task<List<CodeforcesUser>> GetCodeforcesUsersAsync(string handlesString)
        {
            var result = await GetApiDataAsync<JObject>($"https://codeforces.com/api/user.info?handles={handlesString}");

            var users = result["result"]?.ToObject<List<CodeforcesUser>>()
                        ?? throw new InvalidOperationException("Unable to deserialize 'result' to List<CodeforcesUser>.");

            return users;
        }

        public async Task<(List<CodeforcesProblem> Problems, List<CodeforcesProblemStatistics> ProblemStatistics)> GetCodeforcesProblemsAsync()
        {
            var result = await GetApiDataAsync<JObject>("https://codeforces.com/api/problemset.problems");

            var resultData = result["result"] ?? throw new InvalidOperationException("API response does not contain a 'result' field.");

            var problems = resultData["problems"]?.ToObject<List<CodeforcesProblem>>() ?? [];
            var problemStatistics = resultData["problemStatistics"]?.ToObject<List<CodeforcesProblemStatistics>>() ?? [];

            return (problems, problemStatistics);
        }

        public async Task<List<CodeforcesContest>> GetCodeforcesContestsAsync(bool gym)
        {
            var result = await GetApiDataAsync<JObject>($"https://codeforces.com/api/contest.list?gym={gym}");

            return result["result"]?.ToObject<List<CodeforcesContest>>() ?? [];
        }

        public async Task<List<CodeforcesSubmission>> GetCodeforcesSubmissionsAsync(string handle)
        {
            var result = await GetApiDataAsync<JObject>($"https://codeforces.com/api/user.status?handle={handle}");

            return result["result"]?.ToObject<List<CodeforcesSubmission>>() ?? new List<CodeforcesSubmission>();
        }

        public async Task<List<CodeforcesSubmission>> GetCodeforcesContestSubmissionsAsync(string handle, int contestId)
        {
            var result = await GetApiDataAsync<JObject>($"https://codeforces.com/api/contest.status?contestId={contestId}&handle={handle}");

            return result["result"]?.ToObject<List<CodeforcesSubmission>>() ?? new List<CodeforcesSubmission>();
        }

        public async Task<List<string>> GetCodeforcesContestUsersAsync(string[] handles, int contestId)
        {
            var handlesString = string.Join(";", handles);

            var result = await GetApiDataAsync<JObject>(
                $"https://codeforces.com/api/contest.standings?&showUnofficial=true&contestId={contestId}&handles={handlesString}");

            var rows = result["result"]?["rows"]?.ToObject<List<CodeforcesRanklistRow>>() ?? [];

            return rows
                .SelectMany(row => row.Party.Members)
                .Select(member => member.Handle)
                .Distinct()
                .ToList();
        }

        private async Task<T> GetApiDataAsync<T>(string url)
        {
            try
            {
                using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                using var stream = await response.Content.ReadAsStreamAsync();
                using var streamReader = new StreamReader(stream);
                using var jsonReader = new JsonTextReader(streamReader);

                var serializer = new JsonSerializer();
                return serializer.Deserialize<T>(jsonReader)
                       ?? throw new InvalidOperationException("Unable to deserialize response.");
            }
            catch (HttpRequestException e)
            {
                throw new InvalidOperationException($"HTTP error: {e.Message}");
            }
            catch (JsonException e)
            {
                throw new InvalidOperationException($"JSON deserialization error: {e.Message}");
            }
        }
    }
}