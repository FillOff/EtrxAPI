using Etrx.Domain.Models;

namespace Etrx.Application.Repositories;

public interface ITagsRepository : IGenericRepository<Tag>
{
    Task<List<Tag>> GetAllWithTrackingAsync();
    Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}