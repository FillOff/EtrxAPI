using Etrx.Domain.Dtos.Submissions;
using Etrx.Domain.Interfaces;
using Etrx.Domain.Models;
using Etrx.Domain.Queries;
using Etrx.Persistence.Databases;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Etrx.Persistence.Repositories;

public class SubmissionsRepository : GenericRepository<Submission, ulong>, ISubmissionsRepository
{
    public SubmissionsRepository(EtrxDbContext context)
        : base(context)
    { }

    public override async Task<List<Submission>> GetAllAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Include(s => s.User)
            .ToListAsync();
    }

    public async Task<List<Submission>> GetByContestIdAsync(int contestId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(s => s.ContestId == contestId)
            .ToListAsync();
    }

    public async Task<List<string>> GetUserParticipantTypesAsync(string handle)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(s => s.User)
            .Where(s => s.User.Handle == handle)
            .Select(s => s.ParticipantType)
            .Distinct()
            .ToListAsync();
    }

    public async Task<List<GetGroupSubmissionsProtocolResponseDto>> GetGroupProtocolWithSortAsync(GroupProtocolQueryParameters parameters)
    {
        // Filter by unix time
        var query = _dbSet
            .AsNoTracking()
            .Where(s => 
                s.CreationTimeSeconds >= parameters.UnixFrom && 
                s.CreationTimeSeconds <= parameters.UnixTo);

        // Filter by ContestId
        if (parameters.ContestId != null)
        {
            query = query.Where(s => s.ContestId == parameters.ContestId);
        }

        // Format data to GetGroupSubmissionsProtocolResponseDto
        var groupedData = query
            .Where(s => s.Verdict == "OK")
            .GroupBy(s => new { s.User.Handle, s.User.LastName, s.User.FirstName, s.ContestId })
            .Select(g => new GetGroupSubmissionsProtocolResponseDto
            {
                Handle = g.Key.Handle,
                UserName = g.Key.LastName + " " + g.Key.FirstName,
                ContestId = g.Key.ContestId,
                SolvedCount = g.Select(s => s.Index).Distinct().Count()
            });

        //Sorting
        string order = parameters.Sorting.SortOrder == true ? "asc" : "desc";
        groupedData = groupedData.OrderBy($"{parameters.Sorting.SortField} {order}");

        return await groupedData.ToListAsync();
    }

    public async Task<List<Submission>> GetByHandleAndContestIdAsync(HandleContestProtocolQueryParameters parameters)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(s => s.User)
            .Where(s =>
                s.User.Handle == parameters.Handle &&
                s.ContestId == parameters.ContestId &&
                s.CreationTimeSeconds >= parameters.UnixFrom &&
                s.CreationTimeSeconds <= parameters.UnixTo)
            .ToListAsync();
    }
}