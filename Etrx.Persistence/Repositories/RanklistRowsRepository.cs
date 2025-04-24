using Etrx.Domain.Models;
using Etrx.Persistence.Databases;
using Etrx.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Etrx.Persistence.Repositories;

public class RanklistRowsRepository : GenericRepository<RanklistRow, object>, IRanklistRowsRepository
{
    public RanklistRowsRepository(EtrxDbContext context) : base(context)
    { }

    public override IQueryable<RanklistRow> GetAll()
    {
        return base.GetAll()
            .Include(rr => rr.ProblemResults);
    }
}
