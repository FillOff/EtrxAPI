using Etrx.Domain.Dtos.ProblemResults;
using Etrx.Domain.Dtos.RanklistRows;
using Etrx.Domain.Interfaces;
using Etrx.Domain.Models;
using Etrx.Domain.Queries;
using Etrx.Persistence.Databases;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Etrx.Persistence.Repositories;

public class RanklistRowsRepository : GenericRepository<RanklistRow>, IRanklistRowsRepository
{
    public RanklistRowsRepository(EtrxDbContext context) : base(context)
    { }

    public override async Task<List<RanklistRow>> GetAllAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Include(rr => rr.ProblemResults)
            .ToListAsync();
    }

    public async Task<List<RanklistRow>> GetByContestIdAsync(int contestId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(rr => rr.ProblemResults)
            .Where(rr => rr.ContestId == contestId)
            .ToListAsync();
    }

    public async Task<List<GetRanklistRowsResponseDto>> GetByContestIdWithSortAndFilterAsync(RanklistQueryParameters parameters)
    {
        var query = _context.RanklistRows
           .AsNoTracking()
           .Where(rr => rr.ContestId == parameters.ContestId);

        // Filter by participant type
        if (parameters.ParticipantType != "ALL")
        {
            query = query.Where(rr => rr.ParticipantType == parameters.ParticipantType);
        }

        // Joining ranklist rows and users tables
        var combinedQuery = query.Join(
            _context.Users.AsNoTracking(),
            ranklistRow => ranklistRow.Handle,
            user => user.Handle,
            (ranklistRow, user) => new GetRanklistRowsResponseDto
            {
                ContestId = ranklistRow.ContestId,
                Handle = ranklistRow.Handle,
                LastSubmissionTimeSeconds = ranklistRow.LastSubmissionTimeSeconds,
                ParticipantType = ranklistRow.ParticipantType,
                Penalty = ranklistRow.Penalty,
                Points = ranklistRow.Points,
                ProblemResults = ranklistRow.ProblemResults
                    .Select(p => new GetProblemResultsResponseDto(p.Index, p.Points, p.Penalty, p.RejectedAttemptCount, p.Type, p.BestSubmissionTimeSeconds))
                    .ToList(),
                Rank = ranklistRow.Rank,
                SuccessfulHackCount = ranklistRow.SuccessfulHackCount,
                UnsuccessfulHackCount = ranklistRow.UnsuccessfulHackCount,
                Username = user.LastName + " " + user.FirstName,
                City = user.City,
                Organization = user.Organization,
                Grade = user.Grade,
                SolvedCount = ranklistRow.ProblemResults.Count(pr => pr.Points != 0)
            }
        );

        // Sorting
        string order = parameters.Sorting.SortOrder == true ? "asc" : "desc";
        combinedQuery = combinedQuery.OrderBy($"{parameters.Sorting.SortField} {order}");

        return await combinedQuery.ToListAsync();
    }
}
