using Etrx.Domain.Models;

namespace Etrx.Application.Repositories;

public interface ITagRepository : IGenericRepository<Tag>
{
    Task<List<Tag>> GetByNamesAsync(List<string> names);
}