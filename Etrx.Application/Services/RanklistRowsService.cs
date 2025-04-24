using System.Linq.Dynamic.Core;
using Etrx.Application.Interfaces;
using Etrx.Domain.Contracts.RanklistRows;
using Etrx.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Etrx.Application.Services;

public class RanklistRowsService : IRanklistRowsService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IRanklistRowsRepository _ranklistRowsRepository;

    public RanklistRowsService(
        IUsersRepository usersRepository,
        IRanklistRowsRepository ranklistRowsRepository)
    {
        _usersRepository = usersRepository;
        _ranklistRowsRepository = ranklistRowsRepository;
    }

    public async Task<GetRanklistRowsResponseWithPropsDto> GetRanklistRowsWithSortAsync(int contestId, GetRanklistRowsRequestDto dto)
    {
        if (string.IsNullOrEmpty(dto.SortField) ||
            !typeof(GetRanklistRowsResponseDto).GetProperties().Any(p => p.Name.Equals(dto.SortField, StringComparison.InvariantCultureIgnoreCase)))
        {
            throw new Exception($"Invalid field: SortField");
        }

        string order = dto.SortOrder == true ? "asc" : "desc";

        var ranklistRowsQuery = _ranklistRowsRepository.GetAll()
            .Where(rr => rr.ContestId == contestId);

        if (!dto.SortField.Contains("username", StringComparison.InvariantCultureIgnoreCase))
        {
            ranklistRowsQuery = ranklistRowsQuery.OrderBy($"{dto.SortField} {order}");
        }

        if (dto.FilterByParticipantType != "ALL")
        {
            ranklistRowsQuery = ranklistRowsQuery.Where(rr => rr.ParticipantType == dto.FilterByParticipantType);
        }

        var ranklistRows = await ranklistRowsQuery.ToListAsync();

        var rowsResponse = new List<GetRanklistRowsResponseDto>();
        foreach (var row in ranklistRows)
        {
            var user = await _usersRepository.GetByHandle(row.Handle);
            var rowResponse = new GetRanklistRowsResponseDto()
            {
                ContestId = row.ContestId,
                Handle = row.Handle,
                LastSubmissionTimeSeconds = row.LastSubmissionTimeSeconds,
                ParticipantType = row.ParticipantType,
                Penalty = row.Penalty,
                Points = row.Points,
                ProblemResults = row.ProblemResults,
                Rank = row.Rank,
                SuccessfulHackCount = row.SuccessfulHackCount,
                UnsuccessfulHackCount = row.UnsuccessfulHackCount,
                Username = user!.LastName + " " + user!.FirstName,
            };

            rowsResponse.Add(rowResponse);
        }

        if (dto.SortField.Contains("username", StringComparison.InvariantCultureIgnoreCase))
        {
            if (dto.SortOrder == true)
            {
                rowsResponse = rowsResponse.OrderBy(r => r.Username).ToList();
            }
            else
            {
                rowsResponse = rowsResponse.OrderByDescending(r => r.Username).ToList();
            }
        }

        return new GetRanklistRowsResponseWithPropsDto(
            rowsResponse,
            typeof(GetRanklistRowsResponseDto).GetProperties().Select(p => p.Name).ToArray());
    }
}
