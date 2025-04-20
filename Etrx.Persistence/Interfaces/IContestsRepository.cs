using Etrx.Domain.Models;

namespace Etrx.Persistence.Interfaces;

public interface IContestsRepository : IGenericRepository<Contest, int>
{
    new IQueryable<Contest> GetAll();
    new Task<Contest?> GetByKey(int key);
}