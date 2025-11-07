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
           .Include(rr => rr.ProblemResults)
           .Where(rr => rr.ContestId == parameters.ContestId);

        // Filter by participant type
        if (parameters.ParticipantType != "ALL")
        {
            query = query.Where(rr => rr.ParticipantType == parameters.ParticipantType);
        }

        // Join with Users and select the raw data needed
        var combinedQuery = query.Join(
            _context.Users.AsNoTracking(),
            ranklistRow => ranklistRow.Handle,
            user => user.Handle,
            (ranklistRow, user) => new 
            {
                RanklistRowData = ranklistRow,
                UserData = user
            }
        );

        var resultsFromDb = await combinedQuery.ToListAsync();

        var finalResponse = resultsFromDb.Select(data => new GetRanklistRowsResponseDto
        {
            ContestId = data.RanklistRowData.ContestId,
            Handle = data.RanklistRowData.Handle,
            LastSubmissionTimeSeconds = data.RanklistRowData.LastSubmissionTimeSeconds,
            ParticipantType = data.RanklistRowData.ParticipantType,
            Penalty = data.RanklistRowData.Penalty,
            Points = data.RanklistRowData.Points,
            Rank = data.RanklistRowData.Rank,
            SuccessfulHackCount = data.RanklistRowData.SuccessfulHackCount,
            UnsuccessfulHackCount = data.RanklistRowData.UnsuccessfulHackCount,
            Username = data.UserData.LastName + " " + data.UserData.FirstName,
            City = data.UserData.City,
            Organization = data.UserData.Organization,
            Grade = data.UserData.Grade,
            SolvedCount = data.RanklistRowData.ProblemResults.Count(pr => pr.Points != 0),
            ProblemResults = data.RanklistRowData.ProblemResults
                .Select(p => new GetProblemResultsResponseDto(p.Index, p.Points, p.Penalty, p.RejectedAttemptCount, p.Type, p.BestSubmissionTimeSeconds))
                .OrderBy(p => p.Index)
                .ToList()
        }).ToList();

        return finalResponse;
    }
}
