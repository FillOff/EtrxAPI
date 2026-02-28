using Etrx.Application.Constants;
using Etrx.Application.Dtos.Submissions;
using Etrx.Application.Queries;
using Etrx.Application.Repositories;
using Etrx.Domain.Models;
using Etrx.Persistence.Databases;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Etrx.Persistence.Repositories;

public class SubmissionsRepository : GenericRepository<Submission>, ISubmissionsRepository
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

    public async Task<List<GetUsersProtocolsResponseDto>> GetUsersProtocolAsync(long unixFrom, long unixTo, int? contestId)
    {
        // Filter by unix time
        var query = _dbSet
            .AsNoTracking()
            .Where(s => 
                s.CreationTimeSeconds >= unixFrom && 
                s.CreationTimeSeconds <= unixTo);

        // Filter by ContestId
        if (contestId != null)
        {
            query = query.Where(s => s.ContestId == contestId);
        }

        // Format data to GetUsersProtocolsResponseDto
        var groupedData = query
            .GroupBy(s => new { s.User.Handle, s.User.LastName, s.User.FirstName })
            .Select(g => new GetUsersProtocolsResponseDto
            {
                Handle = g.Key.Handle,
                UserName = g.Key.LastName + " " + g.Key.FirstName,
                ContestsCount = g
                    .Select(s => s.ContestId)
                    .Distinct()
                    .Count(),
                SolvedCount = g
                    .Where(s => s.Verdict == Verdicts.Ok)
                    .Select(s => s.ContestId.ToString() + "_" + s.Index)
                    .Distinct()
                    .Count(),
                LastTime = g
                    .Max(g => g.CreationTimeSeconds)
            })
            .OrderByDescending(res => res.LastTime);

        return await groupedData.ToListAsync();
    }

    public async Task<List<GetUserProtocolResponseDto>> GetUserProtocolAsync(string handle, long unixFrom, long unixTo)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(s => s.User)
            .Where(s =>
                s.User.Handle == handle &&
                s.CreationTimeSeconds >= unixFrom &&
                s.CreationTimeSeconds <= unixTo)
            .GroupBy(s => s.ContestId)
            .Select(g => new GetUserProtocolResponseDto
            {
                ContestId = g.Key,
                SolvedCount = g
                    .Where(s => s.Verdict == Verdicts.Ok)
                    .Select(s => s.Index)
                    .Distinct()
                    .Count(),
                LastTime = g.Max(g => g.CreationTimeSeconds)
            })
            .OrderByDescending(dto => dto.LastTime)
            .ThenByDescending(dto => dto.ContestId)
            .ToListAsync();
    }

    public async Task<List<Submission>> GetUserContestProtocolAsync(string handle, int contestId, long unixFrom, long unixTo)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(s => s.User)
            .Where(s =>
                s.User.Handle == handle &&
                s.ContestId == contestId &&
                s.CreationTimeSeconds >= unixFrom &&
                s.CreationTimeSeconds <= unixTo)
            .OrderBy(s => s.Index)
            .ThenByDescending(s => s.CreationTimeSeconds)
            .ToListAsync();
    }

    public async Task<List<Submission>> GetBySubmissionIdsAsync(List<ulong> submissionIds)
    {
        if (submissionIds == null || submissionIds.Count == 0)
        {
            return [];
        }

        return await _dbSet
            .AsNoTracking()
            .Where(s => submissionIds.Contains(s.SubmissionId))
            .ToListAsync();
    }

}