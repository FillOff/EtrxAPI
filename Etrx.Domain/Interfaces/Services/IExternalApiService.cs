using Etrx.Domain.Parsing_models;

namespace Etrx.Domain.Interfaces.Services
{
    public interface IExternalApiService
    {
        Task<(List<DlUser>? Users, string Error)> GetDlUsersAsync();
        Task<(List<CodeforcesUser>? Users, string Error)> GetCodeforcesUsersAsync(string handlesString);
        Task<(List<CodeforcesProblem>? Problems, List<CodeforcesProblemStatistics>? ProblemStatistics, string Error)> GetCodeforcesProblemsAsync();
        Task<(List<CodeforcesContest>? Contests, string Error)> GetCodeforcesContestsAsync(bool gym);
        Task<(List<CodeforcesSubmission>? Submissions, string Error)> GetCodeforcesSubmissionsAsync(string handle);
        Task<(List<CodeforcesSubmission>? Submissions, string Error)> GetCodeforcesContestSubmissionsAsync(string handle, int contestId);
        Task<(List<string> Handles, string Error)> GetCoodeforcesContestUsersAsync(string[] handles, int contestId);
    }
}