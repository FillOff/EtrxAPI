using Etrx.Domain.Models;
using Etrx.Domain.Queries.Common;

namespace Etrx.Domain.Interfaces;

public interface IUsersRepository : IGenericRepository<User, string>
{
    Task<User?> GetByHandleAsync(string handle);
    Task<List<string>> GetHandlesAsync();
    Task<List<User>> GetWithSortAsync(SortingQueryParameters parameters);
}