using Etrx.Domain.Models;

namespace Etrx.Domain.Interfaces.Services
{
    public interface IProblemsService
    {
        Task<List<Problem>> GetAllProblemsAsync();
        Task<Problem?> GetProblemByContestIdAndIndexAsync(
            int contestId,
            string index);
        Task<List<Problem>> GetProblemsByContestIdAsync(int contestId);
        Task<(List<Problem> Problems, int PageCount)> GetProblemsByPageWithSortAndFilterTagsAsync(
            int page,
            int pageSize,
            string? tags,
            string? indexes,
            string? problemName,
            string sortField,
            bool sortOrder,
            int minRating,
            int maxRating,
            double minPoints,
            double maxPoints);
        Task<List<string>> GetAllTagsAsync();
        Task<List<string>> GetAllIndexesAsync();
        Task<List<string>> GetProblemsIndexesByContestIdAsync(int contestId);
        Task<int> CreateProblemAsync(Problem problem);
        Task<int> UpdateProblemAsync(Problem problem);
        Task<int> DeleteProblemAsync(int id);
    }
}