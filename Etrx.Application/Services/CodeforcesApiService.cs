using Etrx.Domain.Models.ParsingModels.Codeforces;
using Etrx.Application.Interfaces;

namespace Etrx.Application.Services;

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

    public async Task<(List<CodeforcesProblem> Problems, List<CodeforcesProblemStatistics> ProblemStatistics)> GetCodeforcesProblemsAsync(string lang)
    {
        var response = await _apiService.GetApiDataAsync<CodeforcesResponse<CodeforcesProblemSetResult>>(
            $"https://codeforces.com/api/problemset.problems?lang={lang}");

        if (response.Result == null)
        {
            throw new Exception(response.Comment);
        }

        return (
            response.Result.Problems,
            response.Result.ProblemStatistics
        );
    }

   public async Task<List<CodeforcesContest>> GetCodeforcesContestsAsync(bool gym, string lang)
    {
        var response = await _apiService.GetApiDataAsync<CodeforcesResponse<List<CodeforcesContest>>>(
            $"https://codeforces.com/api/contest.list?gym={gym}&lang={lang}");

        if (response.Result == null)
        {
            throw new Exception(response.Comment);
        }

        return response.Result;
    }

    public async Task<List<CodeforcesSubmission>> GetCodeforcesSubmissionsAsync(string handle)
    {
        var response = await _apiService.GetApiDataAsync<CodeforcesResponse<List<CodeforcesSubmission>>>(
            $"https://codeforces.com/api/user.status?handle={handle}");

        if (response.Result == null)
        {
            throw new Exception(response.Comment);
        }

        return response.Result;
    }

    public async Task<List<CodeforcesSubmission>> GetCodeforcesContestSubmissionsAsync(string handle, int contestId)
    {
        var response = await _apiService.GetApiDataAsync<CodeforcesResponse<List<CodeforcesSubmission>>>(
            $"https://codeforces.com/api/contest.status?contestId={contestId}&handle={handle}");

        if (response.Result == null)
        {
            throw new Exception(response.Comment);
        }

        return response.Result;
    }

    public async Task<List<string>> GetCodeforcesContestUsersAsync(List<string> handles, int contestId)
    {
        var handlesString = string.Join(";", handles);

        var response = await _apiService.GetApiDataAsync<CodeforcesResponse<CodeforcesContestStanding>>(
            $"https://codeforces.com/api/contest.standings?&showUnofficial=true&contestId={contestId}&handles={handlesString}");

        if (response.Result == null)
        {
            throw new Exception(response.Comment);
        }

        return response.Result.Rows
            .SelectMany(row => row.Party.Members)
            .Select(member => member.Handle)
            .Distinct()
            .ToList();
    }

    public async Task<CodeforcesContestStanding> GetCodeforcesRanklistRowsAsync(List<string> handles, int contestId)
    {
        var handlesString = string.Join(";", handles);

        var response = await _apiService.GetApiDataAsync<CodeforcesResponse<CodeforcesContestStanding>>(
            $"https://codeforces.com/api/contest.standings?&showUnofficial=true&handles={handlesString}&contestId={contestId}");

        if (response.Result == null)
        {
            throw new Exception(response.Comment);
        }

        return response.Result;
    }
}