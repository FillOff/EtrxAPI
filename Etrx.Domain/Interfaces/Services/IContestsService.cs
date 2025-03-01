using Etrx.Domain.Models;

namespace Etrx.Domain.Interfaces.Services
{
    public interface IContestsService
    {
        Task<List<Contest>> GetAllContestsAsync();
        Task<Contest?> GetContestByIdAsync(int contestId);
        Task<(List<Contest> Contests, int PageCount)> GetContestsByPageWithSortAsync(
            int page,
            int pageSize,
            bool? gym,
            string sortField = "contestid",
            bool sortOrder = true);
        Task<int> CreateContestAsync(Contest contest);
        Task<int> UpdateContestAsync(Contest contest);
    }
}