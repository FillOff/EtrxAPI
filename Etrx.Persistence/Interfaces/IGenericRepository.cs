namespace Etrx.Persistence.Interfaces;

public interface IGenericRepository<TEntity, TKey> 
    where TEntity : class
{
    Task Add(TEntity entity);
    Task Delete(TKey id);
    IQueryable<TEntity> GetAll();
    Task<TEntity?> GetByKey(TKey id);
    Task Update(TEntity entity);
    Task InsertOrUpdate(List<TEntity> entities);
}