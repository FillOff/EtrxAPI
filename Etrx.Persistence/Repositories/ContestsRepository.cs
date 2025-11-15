using AutoMapper;
using AutoMapper.QueryableExtensions;
using Etrx.Application.Dtos.Common;
using Etrx.Application.Queries.Common;
using Etrx.Application.Repositories;
using Etrx.Application.Specifications;
using Etrx.Domain.Models;
using Etrx.Persistence.Databases;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Etrx.Persistence.Repositories;

public class ContestsRepository : GenericRepository<Contest>, IContestsRepository
{
    private readonly IMapper _mapper;

    public ContestsRepository(EtrxDbContext context, IMapper mapper)
        : base(context)
    { 
        _mapper = mapper;    
    }

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

    public async Task<PagedResultDto<TResult>> GetPagedAsync<TResult>(
        BaseSpecification<Contest> spec,
        PaginationQueryParameters pagination,
        string lang)
    {
        var query = _dbSet
            .AsNoTracking()
            .AsExpandable();

        query = ApplySpecification(spec, query);

        var projectedQuery = query.ProjectTo<TResult>(_mapper.ConfigurationProvider, new { lang });

        var totalCount = await projectedQuery.CountAsync();

        var items = await projectedQuery
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PagedResultDto<TResult>
        {
            Items = items,
            TotalItemsCount = totalCount,
            TotalPagesCount = (totalCount > 0) ? (int)Math.Ceiling(totalCount / (double)pagination.PageSize) : 0
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
