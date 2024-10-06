using Etrx.Domain.Models;

namespace Etrx.Core.Interfaces.Repositories
{
    public interface IContestsRepository
    {
        Task<int> Create(Contest contest);
        Task<IEnumerable<Contest>> Get();
        Task<Contest?> GetById(int id);
        Task<int> Update(Contest contest);
        Task<int> Delete(int id);
    }
}