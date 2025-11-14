using Etrx.Application.Dtos.Common;
using Etrx.Application.Queries.Common;
using Etrx.Application.Specifications;
using Etrx.Domain.Models;

namespace Etrx.Application.Repositories;

public interface IContestsRepository : IGenericRepository<Contest>
{
    new Task<List<Contest>> GetAllAsync();
    Task<Contest?> GetByContestIdAsync(int key);
    Task<List<Contest>> GetLast10Async();
    Task<List<Contest>> GetByContestIdsAsync(List<int> contestIds);
    Task<PagedResultDto<TResult>> GetPagedAsync<TResult>(BaseSpecification<Contest> spec, PaginationQueryParameters pagination, string lang);
}