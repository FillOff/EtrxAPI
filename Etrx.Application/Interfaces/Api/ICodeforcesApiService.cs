using Etrx.Domain.Models.ParsingModels.Codeforces;

namespace Etrx.Application.Interfaces.Api;

public interface ICodeforcesApiService
{
    Task<List<CodeforcesUser>> GetCodeforcesUsersAsync(string handlesString);
    Task<CodeforcesProblemSetResult> GetCodeforcesProblemsAsync(string lang);
    Task<List<CodeforcesContest>> GetCodeforcesContestsAsync(bool gym, string lang);
    Task<List<CodeforcesSubmission>> GetCodeforcesSubmissionsAsync(string handle);
    Task<List<CodeforcesSubmission>> GetCodeforcesContestSubmissionsAsync(string handle, int contestId);
    Task<List<string>> GetCodeforcesContestUsersAsync(List<string> handles, int contestId, bool isGym);
    Task<CodeforcesContestStanding> GetCodeforcesRanklistRowsAsync(List<string> handles, int contestId, bool isGym);
}