using Etrx.Domain.Interfaces;
using Etrx.Domain.Models;
using Etrx.Persistence.Databases;
using Microsoft.EntityFrameworkCore;

namespace Etrx.Persistence.Repositories;

public class ProblemTranslationsRepository : GenericRepository<ProblemTranslation>, IProblemTranslationsRepository
{
    public ProblemTranslationsRepository(EtrxDbContext context)
        : base(context)
    { }

    public async Task<List<ProblemTranslation>> GetByProblemIdsAndLanguageAsync(List<Guid> problemIds, string languageCode)
    {
        if (problemIds == null || problemIds.Count == 0)
        {
            return [];
        }

        return await _dbSet
            .AsNoTracking()
            .Where(pt => 
                pt.LanguageCode == languageCode && 
                problemIds.Contains(pt.ProblemId))
            .ToListAsync();
    }
}
