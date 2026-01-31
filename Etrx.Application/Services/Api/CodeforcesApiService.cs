using Etrx.Domain.Models.ParsingModels.Codeforces;
using Etrx.Application.Interfaces.Api;
using Etrx.Application.Exceptions;

namespace Etrx.Application.Services.Api;

public class CodeforcesApiService : ICodeforcesApiService
{
    private readonly IApiService _apiService;

    public CodeforcesApiService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<List<CodeforcesUser>> GetCodeforcesUsersAsync(string handlesString)
    {
        var result = await HandleRequestAsync<List<CodeforcesUser>>(
            $"https://codeforces.com/api/user.info?handles={handlesString}&lang=ru&checkHistoricHandles=true");

        return result;
    }

    public async Task<(List<CodeforcesProblem> Problems, List<CodeforcesProblemStatistics> ProblemStatistics)> GetCodeforcesProblemsAsync(string lang)
    {
        var result = await HandleRequestAsync<CodeforcesProblemSetResult>(
            $"https://codeforces.com/api/problemset.problems?lang={lang}");

        return (
            result.Problems,
            result.ProblemStatistics
        );
    }

   public async Task<List<CodeforcesContest>> GetCodeforcesContestsAsync(bool gym, string lang)
    {
        var result = await HandleRequestAsync<List<CodeforcesContest>>(
            $"https://codeforces.com/api/contest.list?gym={gym}&lang={lang}");

        return result;
    }

    public async Task<List<CodeforcesSubmission>> GetCodeforcesSubmissionsAsync(string handle)
    {
        var result = await HandleRequestAsync<List<CodeforcesSubmission>>(
            $"https://codeforces.com/api/user.status?handle={handle}");

        return result;
    }

    public async Task<List<CodeforcesSubmission>> GetCodeforcesContestSubmissionsAsync(string handle, int contestId)
    {
        var result = await HandleRequestAsync<List<CodeforcesSubmission>>(
            $"https://codeforces.com/api/contest.status?contestId={contestId}&handle={handle}");

        return result;
    }

    public async Task<List<string>> GetCodeforcesContestUsersAsync(List<string> handles, int contestId)
    {
        var handlesString = string.Join(";", handles);

        var result = await HandleRequestAsync<CodeforcesContestStanding>(
            $"https://codeforces.com/api/contest.standings?&showUnofficial=true&contestId={contestId}&handles={handlesString}");

        return result.Rows
            .SelectMany(row => row.Party.Members)
            .Select(member => member.Handle)
            .Distinct()
            .ToList();
    }

    public async Task<CodeforcesContestStanding> GetCodeforcesRanklistRowsAsync(List<string> handles, int contestId)
    {
        var handlesString = string.Join(";", handles);

        var result = await HandleRequestAsync<CodeforcesContestStanding>(
            $"https://codeforces.com/api/contest.standings?&showUnofficial=true&handles={handlesString}&contestId={contestId}");

        return result;
    }

    private async Task<TResult> HandleRequestAsync<TResult>(string url)
    {
        var response = await _apiService.GetApiDataAsync<CodeforcesResponse<TResult>>(url);

        if (response.Result is null)
        {
            throw new CodeforcesApiException(response);
        }

        return response.Result;
    }
}