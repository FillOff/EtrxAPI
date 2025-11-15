using Etrx.Domain.Models;
using Etrx.Application.Queries.Common;

namespace Etrx.Application.Repositories;

public interface IUsersRepository : IGenericRepository<User>
{
    Task<User?> GetByHandleAsync(string handle);
    Task<List<string>> GetHandlesAsync();
    Task<List<User>> GetWithSortAsync(SortingQueryParameters parameters);
}