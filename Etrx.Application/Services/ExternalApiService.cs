using Etrx.Domain.Parsing_models;
using Etrx.Domain.Interfaces.Services;
using Newtonsoft.Json;
using System.Text.Json;
using System.Net;

namespace Etrx.Application.Services
{
    public class ExternalApiService : IExternalApiService
    {
        private readonly HttpClient _httpClient;

        public ExternalApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(List<DlUser>? Users, string Error)> GetDlUsersAsync()
        {
            var response = await _httpClient.GetAsync("https://dl.gsu.by/codeforces/api/students");
            if (!response.IsSuccessStatusCode)
                return (null, "Couldn't get data from Dl.");

            string content = await response.Content.ReadAsStringAsync();
            return (JsonConvert.DeserializeObject<List<DlUser>>(content), string.Empty);
        }

        public async Task<(List<CodeforcesUser>? Users, string Error)> GetCodeforcesUsersAsync(string handlesString)
        {
            var response = await _httpClient.GetAsync($"https://codeforces.com/api/user.info?handles={handlesString}&lang=ru");
            if (!response.IsSuccessStatusCode)
                return (null, JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.GetProperty("comment").ToString());

            string json = await response.Content.ReadAsStringAsync();
            if (json.StartsWith('<'))
                return (null, "Couldn't get data from Codeforces.");

            string content = JsonDocument.Parse(json).RootElement.GetProperty("result").ToString();

            return (JsonConvert.DeserializeObject<List<CodeforcesUser>>(content), string.Empty);
        }

        public async Task<(List<CodeforcesProblem>? Problems, List<CodeforcesProblemStatistics>? ProblemStatistics, string Error)> GetCodeforcesProblemsAsync()
        {
            var response = await _httpClient.GetAsync("https://codeforces.com/api/problemset.problems");
            if (!response.IsSuccessStatusCode)
                return (null, null, JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.GetProperty("comment").ToString());

            string json = await response.Content.ReadAsStringAsync();
            if (json.StartsWith('<'))
                return (null, null, "Couldn't get data from Codeforces.");

            string problems = JsonDocument.Parse(json).RootElement.GetProperty("result").GetProperty("problems").ToString();
            string problemStatistics = JsonDocument.Parse(json).RootElement.GetProperty("result").GetProperty("problemStatistics").ToString();

            return (JsonConvert.DeserializeObject<List<CodeforcesProblem>>(problems),
                    JsonConvert.DeserializeObject<List<CodeforcesProblemStatistics>>(problemStatistics),
                    string.Empty);
        }

        public async Task<(List<CodeforcesContest>? Contests, string Error)> GetCodeforcesContestsAsync(bool gym)
        {
            var response = await _httpClient.GetAsync($"https://codeforces.com/api/contest.list?gym={gym}");
            if (!response.IsSuccessStatusCode)
                return (null, JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.GetProperty("comment").ToString());

            string json = await response.Content.ReadAsStringAsync();
            if (json.StartsWith('<'))
                return (null, "Couldn't get data from Codeforces.");

            string contests = JsonDocument.Parse(json).RootElement.GetProperty("result").ToString();

            return (JsonConvert.DeserializeObject<List<CodeforcesContest>>(contests),
                    string.Empty);
        }

        public async Task<(List<CodeforcesSubmission>? Submissions, string Error)> GetCodeforcesSubmissionsAsync(string handle)
        {
            var response = await _httpClient.GetAsync($"https://codeforces.com/api/user.status?handle={handle}");
            if (!response.IsSuccessStatusCode)
                return (null, JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.GetProperty("comment").ToString());

            string json = await response.Content.ReadAsStringAsync();
            if (json.StartsWith('<'))
                return (null, "Couldn't get data from Codeforces.");

            string submissions = JsonDocument.Parse(json).RootElement.GetProperty("result").ToString();

            return (JsonConvert.DeserializeObject<List<CodeforcesSubmission>>(submissions),
                string.Empty);
        }

        public async Task<(List<CodeforcesSubmission>? Submissions, string Error)> GetCodeforcesContestSubmissionsAsync(string handle, int contestId)
        {
            await Task.Delay(2000);
            var response = await _httpClient.GetAsync($"https://codeforces.com/api/contest.status?contestId={contestId}&handle={handle}");
            if (!response.IsSuccessStatusCode)
                return (null, JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.GetProperty("comment").ToString());

            string json = await response.Content.ReadAsStringAsync();
            if (json.StartsWith('<'))
                return (null, "Couldn't get data from Codeforces.");

            string submissions = JsonDocument.Parse(json).RootElement.GetProperty("result").ToString();

            return (JsonConvert.DeserializeObject<List<CodeforcesSubmission>>(submissions),
                string.Empty);
        }

        public async Task<(List<string> Handles, string Error)> GetCoodeforcesContestUsersAsync(string[] handles, int contestId)
        {
            var handlesString = string.Join(";", handles);

            var response = await _httpClient.GetAsync($"https://codeforces.com/api/contest.standings?&showUnofficial=true&contestId={contestId}&handles={handlesString}");
            if (!response.IsSuccessStatusCode)
                return ([], JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.GetProperty("comment").ToString());

            string json = await response.Content.ReadAsStringAsync();
            if (json.StartsWith('<'))
                return ([], "Couldn't get data from Codeforces.");

            string rowsJson = JsonDocument.Parse(json).RootElement.GetProperty("result").GetProperty("rows").ToString();
            var rows = JsonConvert.DeserializeObject<List<CodeforcesRanklistRow>>(rowsJson)!;

            List<string> newHandles = [];
            if (rows.Count != 0)
            {
                newHandles = rows
                    .SelectMany(row => row.Party.Members)
                    .Where(handle => handles.Contains(handle.Handle) && !newHandles.Contains(handle.Handle))
                    .Select(handle => handle.Handle)
                    .Distinct()
                    .ToList();

                return (newHandles, string.Empty);
            }
            else
            {
                return (newHandles, $"There are not users who solve contest {contestId}.");
            }
        }
    }
}