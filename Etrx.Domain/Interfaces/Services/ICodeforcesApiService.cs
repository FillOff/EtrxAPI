using Etrx.Domain.Models.ParsingModels.Dl;
using Etrx.Domain.Models.ParsingModels.Codeforces;

namespace Etrx.Domain.Interfaces.Services
{
    public interface ICodeforcesApiService
    {
        Task<List<DlUser>> GetDlUsersAsync();
        Task<List<CodeforcesUser>> GetCodeforcesUsersAsync(string handlesString);
        Task<(List<CodeforcesProblem> Problems, List<CodeforcesProblemStatistics> ProblemStatistics)> GetCodeforcesProblemsAsync();
        Task<List<CodeforcesContest>> GetCodeforcesContestsAsync(bool gym);
        Task<List<CodeforcesSubmission>> GetCodeforcesSubmissionsAsync(string handle);
        Task<List<CodeforcesSubmission>> GetCodeforcesContestSubmissionsAsync(string handle, int contestId);
        Task<List<string>> GetCodeforcesContestUsersAsync(string[] handles, int contestId);
    }
}