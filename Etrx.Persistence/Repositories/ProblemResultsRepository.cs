using Etrx.Application.Repositories;
using Etrx.Domain.Models;
using Etrx.Persistence.Databases;
using Microsoft.EntityFrameworkCore;

namespace Etrx.Persistence.Repositories;

public class ProblemResultsRepository : GenericRepository<ProblemResult>, IProblemResultsRepository
{
    public ProblemResultsRepository(EtrxDbContext context)
        : base(context)
    { }

    public async Task<List<ProblemResult>> GetByRanklistRowIdsAsync(List<Guid> ranklistRowIds)
    {
        if (ranklistRowIds == null || ranklistRowIds.Count == 0)
        {
            return [];
        }

        return await _dbSet
            .AsNoTracking()
            .Where(pr => ranklistRowIds.Contains(pr.RanklistRowId))
            .ToListAsync();
    }
}
