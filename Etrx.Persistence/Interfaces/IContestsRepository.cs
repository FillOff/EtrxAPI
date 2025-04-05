using Etrx.Domain.Models;
using Etrx.Persistence.Repositories;

namespace Etrx.Persistence.Interfaces
{
    public interface IContestsRepository : IGenericRepository<Contest, int>
    {
        new IQueryable<Contest> GetAll();
        new Task<Contest?> GetByKey(int key);
    }
}