using EFCore.BulkExtensions;
using Etrx.Application.Repositories;
using Etrx.Application.Specifications;
using Etrx.Domain.Models;
using Etrx.Persistence.Databases;
using Microsoft.EntityFrameworkCore;

namespace Etrx.Persistence.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> 
    where TEntity : Entity
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

    public virtual async Task<TEntity?> GetByKeyAsync(Guid key)
    {
        return await _dbSet.FindAsync(key);
    }

    public virtual async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public virtual void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual async Task InsertOrUpdateAsync(List<TEntity> entities)
    {
        await _context.BulkInsertOrUpdateAsync(entities);
    }

    protected static IQueryable<TEntity> ApplySpecification(BaseSpecification<TEntity> spec, IQueryable<TEntity> query)
    {
        if (spec.FilterCondition != null)
        {
            query = query.Where(spec.FilterCondition);
        }

        if (spec.OrderBy != null)
        {
            query = query.OrderBy(spec.OrderBy);
        }
        else if (spec.OrderByDescending != null)
        {
            query = query.OrderByDescending(spec.OrderByDescending);
        }

        return query;
    }
}
