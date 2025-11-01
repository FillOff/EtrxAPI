using Etrx.Domain.Dtos.Common;
using Etrx.Domain.Models;
using Etrx.Domain.Queries;

namespace Etrx.Domain.Interfaces;

public interface IContestsRepository : IGenericRepository<Contest, int>
{
    new Task<List<Contest>> GetAllAsync();
    new Task<Contest?> GetByKeyAsync(int key);
    Task<List<Contest>> GetLast10Async();
    Task<PagedResultDto<Contest>> GetPagedWithSortAndFilterAsync(ContestQueryParameters parameters);
}