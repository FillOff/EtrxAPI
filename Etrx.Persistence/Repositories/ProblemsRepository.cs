using Etrx.Domain.Dtos.Common;
using Etrx.Domain.Interfaces;
using Etrx.Domain.Models;
using Etrx.Domain.Queries;
using Etrx.Persistence.Databases;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Etrx.Persistence.Repositories;

public class ProblemsRepository : GenericRepository<Problem, object>, IProblemsRepository
{
    public ProblemsRepository(EtrxDbContext context)
        : base(context)
    { }

    public override async Task<List<Problem>> GetAllAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Include(p => p.ProblemTranslations)
            .ToListAsync();
    }

    public async Task<Problem?> GetByKeyAsync(int contestId, string index)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(p => p.ProblemTranslations)
            .FirstOrDefaultAsync(p => p.ContestId == contestId && p.Index == index);
    }

    public async Task<List<Problem>> GetByContestIdAsync(int contestId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(p => p.ProblemTranslations)
            .Where(p => p.ContestId == contestId)
            .ToListAsync();
    }

    public async Task<List<string>> GetAllTagsAsync(int minRating, int maxRating)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => 
                p.Tags != null &&
                p.Rating >= minRating &&
                p.Rating <= maxRating)
            .SelectMany(problem => problem.Tags!)
            .Distinct()
            .OrderBy(tag => tag)
            .ToListAsync();
    }

    public async Task<List<string>> GetAllIndexesAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Select(problem => problem.Index)
            .Distinct()
            .OrderBy(index => index)
            .ToListAsync();
    }

    public async Task<List<string>> GetIndexesByContestIdAsync(int contestId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.ContestId == contestId)
            .Select(p => p.Index)
            .ToListAsync();
    }

    public async Task<PagedResultDto<Problem>> GetByPageWithSortAndFilterAsync(ProblemQueryParameters parameters)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(p => p.ProblemTranslations)
            .AsQueryable();

        // Filter by tags
        if (!string.IsNullOrEmpty(parameters.Tags))
        {
            var tagsFilter = parameters.Tags.Split(';', StringSplitOptions.RemoveEmptyEntries);
            if (tagsFilter.Length > 0)
            {
                if (parameters.IsOnly)
                {
                    query = query.Where(
                        p => p.Tags != null && 
                        p.Tags.Count == tagsFilter.Length && 
                        p.Tags.All(t => tagsFilter.Contains(t)));
                }
                else
                {
                    query = query.Where(
                        p => p.Tags != null && 
                        tagsFilter.All(tag => p.Tags.Contains(tag)));
                }
            }
        }

        // Filter by indexes
        if (!string.IsNullOrEmpty(parameters.Indexes))
        {
            var indexesFilter = parameters.Indexes.Split(';', StringSplitOptions.RemoveEmptyEntries);
            if (indexesFilter.Length > 0)
            {
                query = query.Where(p => indexesFilter.Contains(p.Index));
            }
        }

        // Filter by problem name
        if (!string.IsNullOrEmpty(parameters.ProblemName))
        {
            query = query.Where(p => p.ProblemTranslations.Any(
                pt => pt.LanguageCode == parameters.Lang && 
                pt.Name.Contains(parameters.ProblemName)));
        }

        // Filter by rating and points
        query = query.Where(p => p.Rating >= parameters.MinRating && p.Rating <= parameters.MaxRating);
        query = query.Where(p => p.Points >= parameters.MinPoints && p.Points <= parameters.MaxPoints);

        // Sorting
        string order = parameters.Sorting.SortOrder == true ? "asc" : "desc";
        switch (parameters.Sorting.SortField.ToLowerInvariant())
        {
            case "name":
                if (order == "asc")
                {
                    query = query.OrderBy(p => p.ProblemTranslations
                        .FirstOrDefault(t => t.LanguageCode == parameters.Lang)!.Name);
                }
                else
                {
                    query = query.OrderByDescending(p => p.ProblemTranslations
                        .FirstOrDefault(t => t.LanguageCode == parameters.Lang)!.Name);
                }
                break;
            default:
                query = query.OrderBy($"{parameters.Sorting.SortField} {order}");
                break;
        }

        // Calculating total pages count
        int totalCount = await query.CountAsync();
        int totalPages = (totalCount > 0) ? (int)Math.Ceiling((double)totalCount / parameters.Pagination.PageSize) : 0;

        // Pagination
        var items = await query
            .Skip((parameters.Pagination.Page - 1) * parameters.Pagination.PageSize)
            .Take(parameters.Pagination.PageSize)
            .ToListAsync();

        return new PagedResultDto<Problem>
        {
            Items = items,
            TotalPagesCount = totalPages
        };
    }
}