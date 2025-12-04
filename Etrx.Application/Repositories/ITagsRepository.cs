using Etrx.Domain.Models;

namespace Etrx.Application.Repositories;

public interface ITagsRepository : IGenericRepository<Tag>
{
    Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}