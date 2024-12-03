using Etrx.Domain.Models;

namespace Etrx.Domain.Interfaces.Repositories
{
    public interface IContestsRepository
    {
        Task<int> Create(Contest contest);
        IQueryable<Contest> Get();
        Contest? GetById(int contestId);
        Task<int> Update(Contest contest);
        Task<int> Delete(int contestId);


        Task InsertOrUpdateAsync(List<Contest> contests);
    }
}