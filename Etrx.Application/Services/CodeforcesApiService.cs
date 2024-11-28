using Etrx.Domain.Parsing_models;
using Etrx.Domain.Interfaces.Services;
using Newtonsoft.Json;
using System.Text.Json;

namespace Etrx.Application.Services
{
    public class CodeforcesApiService : ICodeforcesApiService
    {
        private readonly HttpClient _httpClient;

        public CodeforcesApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(List<DlUser>? Users, string Error)> GetDlUsersAsync()
        {
            var (isCorrectResponse, content) = await GetResponseAsync(
                _httpClient,
                "https://dl.gsu.by/codeforces/api/students",
                "dl.gsu.by");

            if (!isCorrectResponse)
                return (null, content);

            return (JsonConvert.DeserializeObject<List<DlUser>>(content), string.Empty);
        }

        public async Task<(List<CodeforcesUser>? Users, string Error)> GetCodeforcesUsersAsync(string handlesString)
        {
            var (isCorrectResponse, content) = await GetResponseAsync(
                _httpClient,
                $"https://codeforces.com/api/user.info?handles={handlesString}&lang=ru",
                "codeforces.com");

            if (!isCorrectResponse)
                return (null, content);

            string users = JsonDocument.Parse(content).RootElement.GetProperty("result").ToString();

            return (JsonConvert.DeserializeObject<List<CodeforcesUser>>(users), string.Empty);
        }

        public async Task<(List<CodeforcesProblem>? Problems, List<CodeforcesProblemStatistics>? ProblemStatistics, string Error)> GetCodeforcesProblemsAsync()
        {
            var (isCorrectResponse, content) = await GetResponseAsync(
                _httpClient,
                "https://codeforces.com/api/problemset.problems",
                "codeforces.com");

            if (!isCorrectResponse)
                return (null, null, content);

            string problems = JsonDocument.Parse(content).RootElement.GetProperty("result").GetProperty("problems").ToString();
            string problemStatistics = JsonDocument.Parse(content).RootElement.GetProperty("result").GetProperty("problemStatistics").ToString();

            return (JsonConvert.DeserializeObject<List<CodeforcesProblem>>(problems),
                    JsonConvert.DeserializeObject<List<CodeforcesProblemStatistics>>(problemStatistics),
                    string.Empty);
        }

        public async Task<(List<CodeforcesContest>? Contests, string Error)> GetCodeforcesContestsAsync(bool gym)
        {
            var (isCorrectResponse, content) = await GetResponseAsync(
                _httpClient,
                $"https://codeforces.com/api/contest.list?gym={gym}",
                "codeforces.com");

            if (!isCorrectResponse)
                return (null, content);

            string contests = JsonDocument.Parse(content).RootElement.GetProperty("result").ToString();

            return (JsonConvert.DeserializeObject<List<CodeforcesContest>>(contests),
                    string.Empty);
        }

        public async Task<(List<CodeforcesSubmission>? Submissions, string Error)> GetCodeforcesSubmissionsAsync(string handle)
        {
            var (isCorrectResponse, content) = await GetResponseAsync(
                _httpClient,
                $"https://codeforces.com/api/user.status?handle={handle}",
                "codeforces.com");

            if (!isCorrectResponse)
                return (null, content);
    
            string submissions = JsonDocument.Parse(content).RootElement.GetProperty("result").ToString();
    
            return (JsonConvert.DeserializeObject<List<CodeforcesSubmission>>(submissions),
                string.Empty);
        }

        public async Task<(List<CodeforcesSubmission>? Submissions, string Error)> GetCodeforcesContestSubmissionsAsync(string handle, int contestId)
        {
            await Task.Delay(2000);

            var (isCorrectResponse, content) = await GetResponseAsync(
                _httpClient,
                $"https://codeforces.com/api/contest.status?contestId={contestId}&handle={handle}",
                "codeforces.com");

            if (!isCorrectResponse)
                return (null, content);

            string submissions = JsonDocument.Parse(content).RootElement.GetProperty("result").ToString();

            return (JsonConvert.DeserializeObject<List<CodeforcesSubmission>>(submissions),
                string.Empty);
        }

        public async Task<(List<string>? Handles, string Error)> GetCoodeforcesContestUsersAsync(string[] handles, int contestId)
        {
            var handlesString = string.Join(";", handles);

            var (isCorrectResponse, content) = await GetResponseAsync(
                _httpClient, 
                $"https://codeforces.com/api/contest.standings?&showUnofficial=true&contestId={contestId}&handles={handlesString}", 
                "codeforces.com");

            if (!isCorrectResponse)
                return (null, content);

            string rowsJson = JsonDocument.Parse(content).RootElement.GetProperty("result").GetProperty("rows").ToString();
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

        private static async Task<(bool, string)> GetResponseAsync(HttpClient httpclient, string url, string host)
        {
            try
            {
                var response = await httpclient.GetAsync(url);

                if (host == "dl.gsu.by")
                {
                    if (!response.IsSuccessStatusCode)
                        return (false, $"Couldn't get data from {host}");

                    return (true, await response.Content.ReadAsStringAsync());
                }
                else if (host == "codeforces.com")
                {
                    if (!response.IsSuccessStatusCode)
                        return (false, JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.GetProperty("comment").ToString());

                    string content =  await response.Content.ReadAsStringAsync();
                    if (content.StartsWith('<'))
                        return (false, "Couldn't get data from Codeforces.");

                    return (true, content);
                }
                return (false, $"Couldn't get data from {host}");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }
    }
}