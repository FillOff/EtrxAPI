using System.Linq.Expressions;

namespace Etrx.Application.Specifications;

public abstract class BaseSpecification<TEntity>
{
    public Expression<Func<TEntity, bool>>? FilterCondition { get; protected set; }
    public List<Expression<Func<TEntity, object>>> Includes { get; } = new();
    public Expression<Func<TEntity, object>>? OrderBy { get; protected set; }
    public Expression<Func<TEntity, object>>? OrderByDescending { get; protected set; }
}