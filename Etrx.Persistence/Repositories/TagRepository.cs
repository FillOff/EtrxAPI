using Etrx.Application.Repositories;
using Etrx.Domain.Entities;
using Etrx.Persistence.Databases;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Etrx.Persistence.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly EtrxDbContext _dbContext;

        public TagRepository(EtrxDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Tag?> GetByNameAsync(string name, CancellationToken token)
            => await _dbContext.Tags.FirstOrDefaultAsync(t => t.Name == name, token);

        public async Task<List<Tag>> GetAllAsync(CancellationToken token)
            => await _dbContext.Tags.OrderBy(t => t.Complexity).ToListAsync(token);

        public async Task<Tag?> GetByIdAsync(Guid id, CancellationToken token)
            => await _dbContext.Tags.FindAsync(new object[] { id }, token);

        public async Task CreateAsync(Tag tag, CancellationToken token)
        {
            await _dbContext.Tags.AddAsync(tag, token);
            await _dbContext.SaveChangesAsync(token);
        }

        public async Task UpdateAsync(Tag tag, CancellationToken token)
        {
            _dbContext.Tags.Update(tag);
            await _dbContext.SaveChangesAsync(token);
        }
    }
}