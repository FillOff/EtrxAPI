using EFCore.BulkExtensions;
using Etrx.Domain.Interfaces;
using Etrx.Persistence.Databases;
using Microsoft.EntityFrameworkCore;

namespace Etrx.Persistence.Repositories;

public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> 
    where TEntity : class
{
    protected readonly EtrxDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public GenericRepository(EtrxDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<List<TEntity>> GetAllAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .ToListAsync();
    }

    public virtual async Task<TEntity?> GetByKeyAsync(TKey key)
    {
        return await _dbSet.FindAsync(key);
    }

    public virtual async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(TKey key)
    {
        var entity = await _dbSet.FindAsync(key);

        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public virtual async Task InsertOrUpdateAsync(List<TEntity> entities)
    {
        await _context.BulkInsertOrUpdateAsync(entities);
    }
}
