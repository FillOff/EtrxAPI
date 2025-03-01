using Etrx.Domain.Models;

namespace Etrx.Domain.Interfaces.Repositories
{
    public interface IContestsRepository
    {
        Task<List<Contest>> Get();
        Task<Contest?> GetById(int contestId);
        Task<List<Contest>> GetByPageWithSort(
            int page,
            int pageSize,
            bool? gym,
            string sortField,
            string order);
        Task<int> Create(Contest contest);
        Task InsertOrUpdateAsync(List<Contest> contests);
        Task<int> Update(Contest contest);
        Task<int> Delete(int contestId);
    }
}