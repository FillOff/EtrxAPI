using EFCore.BulkExtensions;
using Etrx.Persistence.Databases;
using Etrx.Persistence.Interfaces;
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

    public virtual IQueryable<TEntity> GetAll()
    {
        return _dbSet.AsNoTracking();
    }

    public virtual async Task<TEntity?> GetByKey(TKey key)
    {
        return await _dbSet.FindAsync(key);
    }

    public virtual async Task Add(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task Update(TEntity entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task Delete(TKey key)
    {
        var entity = await _dbSet.FindAsync(key);

        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public virtual async Task InsertOrUpdate(List<TEntity> entities)
    {
        await _context.BulkInsertOrUpdateAsync(entities);
    }
}
