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

    public async Task<List<Tag>> GetByNamesAsync(List<string> names)
    {
        return await _context.Tags
            .Where(t => names.Contains(t.Name))
            .ToListAsync();
    }
}