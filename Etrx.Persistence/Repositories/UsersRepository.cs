using Etrx.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Etrx.Persistence.Databases;
using Etrx.Domain.Interfaces;
using Etrx.Domain.Queries.Common;
using System.Linq.Dynamic.Core;

namespace Etrx.Persistence.Repositories;

public class UsersRepository : GenericRepository<User>, IUsersRepository
{
    public UsersRepository(EtrxDbContext context)
        : base(context)
    { }

    public async Task<User?> GetByHandleAsync(string handle)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Handle == handle);
    }

    public async Task<List<string>> GetHandlesAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Select(u => u.Handle)
            .ToListAsync();
    }

    public async Task<List<User>> GetWithSortAsync(SortingQueryParameters parameters)
    {
        string order = parameters.SortOrder == true ? "asc" : "desc";

        return await _dbSet
            .AsNoTracking()
            .OrderBy($"{parameters.SortField} {order}")
            .ToListAsync();
    }
}
