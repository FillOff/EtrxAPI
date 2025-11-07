using Etrx.Domain.Dtos.Common;
using Etrx.Domain.Models;
using Etrx.Domain.Queries;

namespace Etrx.Domain.Interfaces;

public interface IContestsRepository : IGenericRepository<Contest>
{
    new Task<List<Contest>> GetAllAsync();
    Task<Contest?> GetByContestIdAsync(int key);
    Task<List<Contest>> GetLast10Async();
    Task<PagedResultDto<Contest>> GetPagedWithSortAndFilterAsync(ContestQueryParameters parameters);
    Task<List<Contest>> GetByContestIdsAsync(List<int> contestIds);
}