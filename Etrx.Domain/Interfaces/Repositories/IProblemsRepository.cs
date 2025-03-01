using Etrx.Domain.Models;

namespace Etrx.Domain.Interfaces.Repositories
{
    public interface IProblemsRepository
    {
        Task<List<Problem>> Get();
        Task<List<Problem>> GetByContestId(int contestId);
        Task<Problem?> GetByContestIdAndIndex(int contestId, string index);
        Task<List<string>> GetAllTags();
        Task<List<string>> GetAllIndexes();
        Task<List<string>> GetIndexesByContestId(int contestId);
        Task<List<Problem>> GetByPageWithSortAndFilterTags(
            int page,
            int pageSize,
            string? tags,
            string? indexes,
            string? problemName,
            string sortField,
            string order);
        Task InsertOrUpdate(List<Problem> problems);
        Task<int> Create(Problem problem);
        Task<int> Update(Problem problem);
        Task<int> Delete(int id);
    }
}