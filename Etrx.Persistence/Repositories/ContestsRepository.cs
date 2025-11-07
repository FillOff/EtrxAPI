using Etrx.Domain.Dtos.Common;
using Etrx.Domain.Interfaces;
using Etrx.Domain.Models;
using Etrx.Domain.Queries;
using Etrx.Persistence.Databases;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Etrx.Persistence.Repositories;

public class ContestsRepository : GenericRepository<Contest>, IContestsRepository
{
    public ContestsRepository(EtrxDbContext context) : base(context)
    { }

    public override async Task<List<Contest>> GetAllAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Include(c => c.ContestTranslations)
            .ToListAsync();
    }

    public async Task<Contest?> GetByContestIdAsync(int key)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(c => c.ContestTranslations)
            .FirstOrDefaultAsync(c => c.ContestId == key);
    }

    public async Task<List<Contest>> GetLast10Async()
    {
        return await _dbSet
            .AsNoTracking()
            .Include(c => c.ContestTranslations)
            .Where(c => c.Phase == "FINISHED" && !c.IsContestLoaded)
            .OrderByDescending(c => c.StartTime)
            .Take(10)
            .ToListAsync();
    }

    public async Task<PagedResultDto<Contest>> GetPagedWithSortAndFilterAsync(ContestQueryParameters parameters)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(c => c.ContestTranslations)
            .Where(c => c.Phase != "BEFORE");

        // Filter by gym
        if (parameters.Gym != null)
        {
            query = query.Where(c => c.Gym == parameters.Gym);
        }

        // Sorting
        string order = parameters.Sorting.SortOrder == true ? "asc" : "desc";
        switch (parameters.Sorting.SortField.ToLowerInvariant())
        {
            case "name":
                if (order == "asc")
                {
                    query = query.OrderBy(c => c.ContestTranslations
                        .FirstOrDefault(t => t.LanguageCode == parameters.Lang)!.Name);
                }
                else
                {
                    query = query.OrderByDescending(c => c.ContestTranslations
                        .FirstOrDefault(t => t.LanguageCode == parameters.Lang)!.Name);
                }
                break;
            default:
                query = query.OrderBy($"{parameters.Sorting.SortField} {order}");
                break;
        }

        // Calculating total pages count
        int totalCount = await query.CountAsync();
        int totalPages = 0;

        if (totalCount > 0)
        {
            totalPages = (int)Math.Ceiling((double)totalCount / parameters.Pagination.PageSize);
        }

        // Pagination
        var items = await query
            .Skip((parameters.Pagination.Page - 1) * parameters.Pagination.PageSize)
            .Take(parameters.Pagination.PageSize)
            .ToListAsync();

        return new PagedResultDto<Contest>
        {
            Items = items,
            TotalPagesCount = totalPages
        };
    }

    public async Task<List<Contest>> GetByContestIdsAsync(List<int> contestIds)
    {
        if (contestIds == null || contestIds.Count == 0)
        {
            return [];
        }

        return await _dbSet
            .AsNoTracking()
            .Where(c => contestIds.Contains(c.ContestId))
            .ToListAsync();
    }

}
