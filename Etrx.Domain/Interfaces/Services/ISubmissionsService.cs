using Etrx.Domain.Models;

namespace Etrx.Domain.Interfaces.Services
{
    public interface ISubmissionsService
    {
        Task<List<Submission>> GetAllSubmissionsAsync();
        Task<List<Submission>> GetSubmissionsByContestIdAsync(int contestId);
        Task<List<string>> GetUserParticipantTypesAsync(string handle);
        (int SolvedCount, List<int> Tries) GetTriesAndSolvedCountByHandleAsync(
            List<Submission> userSubmissions,
            List<string> indexes);
        Task<ulong> CreateSubmissionAsync(Submission submission);
        Task<ulong> UpdateSubmissionAsync(Submission submission);
        Task<ulong> DeleteSubmissionAsync(ulong id);
    }
}