using Etrx.Domain.Dtos.Common;
using Etrx.Domain.Models;
using Etrx.Domain.Queries;

namespace Etrx.Domain.Interfaces;

public interface IProblemsRepository : IGenericRepository<Problem>
{
    new Task<List<Problem>> GetAllAsync();
    Task<Problem?> GetByContestIdAndIndexAsync(int contestId, string index);
    Task<List<Problem>> GetByContestIdAsync(int contestId);
    Task<List<string>> GetAllTagsAsync(int minRating, int maxRating);
    Task<List<string>> GetAllIndexesAsync();
    Task<List<string>> GetIndexesByContestIdAsync(int contestId);
    PagedResultDto<Problem> GetByPageWithSortAndFilter(ProblemQueryParameters parameters);
    Task<List<Problem>> GetByContestAndIndexAsync(List<(int ContestId, string Index)> identifiers);
}