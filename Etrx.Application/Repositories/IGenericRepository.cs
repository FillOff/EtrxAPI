using Etrx.Domain.Models;

namespace Etrx.Application.Repositories;

public interface IGenericRepository<TEntity> 
    where TEntity : Entity
{
    Task<List<TEntity>> GetAllAsync();
    Task AddAsync(TEntity entity);
    Task AddRangeAsync(IList<TEntity> entities);
    void Delete(TEntity entity);
    Task<TEntity?> GetByKeyAsync(Guid id);
    void Update(TEntity entity);
    Task InsertOrUpdateAsync(List<TEntity> entities);
}