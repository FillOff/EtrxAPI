using Etrx.Application.Repositories;
using Etrx.Domain.Models;
using Etrx.Persistence.Databases;
using Microsoft.EntityFrameworkCore;

namespace Etrx.Persistence.Repositories;

public class TagRepository : GenericRepository<Tag>, ITagRepository
{
    public TagRepository(EtrxDbContext context) 
        : base(context)
    { }

    public override Task<List<Tag>> GetAllAsync()
    {
        return _dbSet
            .AsNoTracking()
            .OrderByDescending(t => t.Priority)
            .ToListAsync();
    }

    public async Task<List<Tag>> GetByNamesAsync(List<string> names)
    {
        return await _dbSet
            .Where(t => names.Contains(t.Name))
            .ToListAsync();
    }
}