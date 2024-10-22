using Etrx.Domain.Parsing_models;
using Etrx.Domain.Interfaces.Services;
using Newtonsoft.Json;
using System.Text.Json;
using Etrx.Core.Parsing_models;
using Etrx.Domain.Models;

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
                return (null, "Couldn't get data from Codeforces.");

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
                return (null, null, "Couldn't get data from Codeforces.");

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
                return (null, "Couldn't get data from Codeforces.");

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
                return (null, "Couldn't get data from Codeforces.");

            string json = await response.Content.ReadAsStringAsync();
            if (json.StartsWith('<'))
                return (null, "Couldn't get data from Codeforces.");

            string submissions = JsonDocument.Parse(json).RootElement.GetProperty("result").ToString();

            return (JsonConvert.DeserializeObject<List<CodeforcesSubmission>>(submissions),
                string.Empty);
        }

        public async Task<(List<CodeforcesSubmission>? Submissions, string Error)> GetCodeforcesContestSubmissionsAsync(string handle, int contestId)
        {
            var response = await _httpClient.GetAsync($"https://codeforces.com/api/contest.status?contestId={contestId}&handle={handle}");
            if (!response.IsSuccessStatusCode)
                return (null, "Couldn't get data from Codeforces.");

            string json = await response.Content.ReadAsStringAsync();
            if (json.StartsWith('<'))
                return (null, "Couldn't get data from Codeforces.");

            string submissions = JsonDocument.Parse(json).RootElement.GetProperty("result").ToString();

            return (JsonConvert.DeserializeObject<List<CodeforcesSubmission>>(submissions),
                string.Empty);
        }
    }
}