using Newtonsoft.Json.Linq;
using Etrx.Domain.Models.ParsingModels.Codeforces;
using Etrx.Core.Models.Parsing_models.Codeforces;
using Etrx.Application.Interfaces;

namespace Etrx.Application.Services
{
    public class CodeforcesApiService : ICodeforcesApiService
    {
        private readonly IApiService _apiService;

        public CodeforcesApiService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<CodeforcesUser>> GetCodeforcesUsersAsync(string handlesString)
        {
            var response = await _apiService.GetApiDataAsync<CodeforcesResponse<List<CodeforcesUser>>>(
                $"https://codeforces.com/api/user.info?handles={handlesString}&lang=ru&checkHistoricHandles=true");

            if (response.Result == null)
            {
                throw new Exception(response.Comment);
            }
            
            return response.Result;
        }

        public async Task<(List<CodeforcesProblem> Problems, List<CodeforcesProblemStatistics> ProblemStatistics)> GetCodeforcesProblemsAsync()
        {
            var response = await _apiService.GetApiDataAsync<CodeforcesResponse<CodeforcesProblemsResult>>(
                "https://codeforces.com/api/problemset.problems?lang=ru");

            return (
                response.Result!.Problems,
                response.Result!.ProblemStatistics
            );
        }

        public async Task<List<CodeforcesContest>> GetCodeforcesContestsAsync(bool gym)
        {
            var response = await _apiService.GetApiDataAsync<CodeforcesResponse<List<CodeforcesContest>>>(
                $"https://codeforces.com/api/contest.list?gym={gym}&lang=ru");

            return response.Result ?? [];
        }

        public async Task<List<CodeforcesSubmission>> GetCodeforcesSubmissionsAsync(string handle)
        {
            var response = await _apiService.GetApiDataAsync<CodeforcesResponse<List<CodeforcesSubmission>>>(
                $"https://codeforces.com/api/user.status?handle={handle}");

            return response.Result ?? [];
        }

        public async Task<List<CodeforcesSubmission>> GetCodeforcesContestSubmissionsAsync(string handle, int contestId)
        {
            var response = await _apiService.GetApiDataAsync<CodeforcesResponse<List<CodeforcesSubmission>>>(
                $"https://codeforces.com/api/contest.status?contestId={contestId}&handle={handle}");

            return response.Result ?? [];
        }

        public async Task<List<string>> GetCodeforcesContestUsersAsync(List<string> handles, int contestId)
        {
            var handlesString = string.Join(";", handles);

            var result = await _apiService.GetApiDataAsync<JObject>(
                $"https://codeforces.com/api/contest.standings?&showUnofficial=true&contestId={contestId}&handles={handlesString}");

            var rows = result["result"]?["rows"]?.ToObject<List<CodeforcesRanklistRow>>() ?? [];

            return rows
                .SelectMany(row => row.Party.Members)
                .Select(member => member.Handle)
                .Distinct()
                .ToList();
        }
    }
}