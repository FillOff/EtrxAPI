using Etrx.Application.Constants;
using Etrx.Application.Queries.Common;
using Etrx.Application.Repositories;
using Etrx.Domain.Models;
using Etrx.Persistence.Databases;
using Microsoft.EntityFrameworkCore;
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

    public async Task<bool> DeleteByHandleAsync(string handle)
    {
        return await _dbSet
            .Where(u => u.Handle == handle)
            .ExecuteDeleteAsync() > 0;
    }

    public Task<User?> FindByHandleAsync(string handle, CancellationToken cancellationToken)
    {
        var normalizedHandle = handle.Trim().ToLowerInvariant();
        return _dbSet.AsNoTracking().SingleOrDefaultAsync(user => user.Handle.ToLower() == normalizedHandle, cancellationToken);
    }

    public Task<User?> FindByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _dbSet.AsNoTracking().SingleOrDefaultAsync(user => user.Id == userId, cancellationToken);
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
        string order = parameters.SortOrder == true ? SortOrders.Asc : SortOrders.Desc;

        return await _dbSet
            .AsNoTracking()
            .OrderBy($"{parameters.SortField} {order}")
            .ToListAsync();
    }

    public async Task DeleteAllAsync()
    {
        await _dbSet.ExecuteDeleteAsync();
    }
}
