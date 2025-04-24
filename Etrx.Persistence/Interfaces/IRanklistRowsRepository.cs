using Etrx.Domain.Models;

namespace Etrx.Persistence.Interfaces
{
    public interface IRanklistRowsRepository : IGenericRepository<RanklistRow, object>
    {
        new IQueryable<RanklistRow> GetAll();
    }
}