namespace Etrx.Domain.Interfaces;

public interface IGenericRepository<TEntity, TKey> 
    where TEntity : class
{
    Task<List<TEntity>> GetAllAsync();
    Task AddAsync(TEntity entity);
    Task DeleteAsync(TKey id);
    Task<TEntity?> GetByKeyAsync(TKey id);
    Task UpdateAsync(TEntity entity);
    Task InsertOrUpdateAsync(List<TEntity> entities);
}