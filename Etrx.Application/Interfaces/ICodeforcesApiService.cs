using Etrx.Domain.Models.ParsingModels.Codeforces;

namespace Etrx.Application.Interfaces;

public interface ICodeforcesApiService
{
    Task<List<CodeforcesUser>> GetCodeforcesUsersAsync(string handlesString);
    Task<(List<CodeforcesProblem> Problems, List<CodeforcesProblemStatistics> ProblemStatistics)> GetCodeforcesProblemsAsync(string lang);
    Task<List<CodeforcesContest>> GetCodeforcesContestsAsync(bool gym, string lang);
    Task<List<CodeforcesSubmission>> GetCodeforcesSubmissionsAsync(string handle);
    Task<List<CodeforcesSubmission>> GetCodeforcesContestSubmissionsAsync(string handle, int contestId);
    Task<List<string>> GetCodeforcesContestUsersAsync(List<string> handles, int contestId);
}