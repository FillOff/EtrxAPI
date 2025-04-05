using Etrx.Domain.Models;
using Etrx.Persistence.Databases;
using Etrx.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Etrx.Persistence.Repositories;

public class ContestsRepository : GenericRepository<Contest, int>, IContestsRepository
{
    public ContestsRepository(EtrxDbContext context) : base(context)
    { }

    public override IQueryable<Contest> GetAll()
    {
        return base.GetAll()
            .Include(c => c.ContestTranslations);
    }

    public override async Task<Contest?> GetByKey(int key)
    {
        return await _dbSet
            .Include(c => c.ContestTranslations)
            .FirstOrDefaultAsync(c => c.ContestId == key);
    }
}
