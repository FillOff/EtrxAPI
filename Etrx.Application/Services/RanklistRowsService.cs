using System.Linq.Dynamic.Core;
using AutoMapper;
using Etrx.Application.Interfaces;
using Etrx.Core.Contracts.Problems;
using Etrx.Domain.Contracts.RanklistRows;
using Etrx.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Etrx.Application.Services;

public class RanklistRowsService : IRanklistRowsService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IRanklistRowsRepository _ranklistRowsRepository;
    private readonly IProblemsRepository _problemsRepository;
    private readonly IMapper _mapper;

    public RanklistRowsService(
        IUsersRepository usersRepository,
        IRanklistRowsRepository ranklistRowsRepository,
        IProblemsRepository problemsRepository,
        IMapper mapper)
    {
        _usersRepository = usersRepository;
        _ranklistRowsRepository = ranklistRowsRepository;
        _problemsRepository = problemsRepository;
        _mapper = mapper;
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
                City = user!.City,
                Organization = user!.Organization,
                Grade = user!.Grade,
                SolvedCount = row.ProblemResults.Where(pr => pr.Points != 0).Count(),
            };

            rowsResponse.Add(rowResponse);
        }

        rowsResponse = rowsResponse.AsQueryable()
            .OrderBy($"{dto.SortField} {order}")
            .ToList();

        var problems = await _problemsRepository.GetByContestId(contestId).ToListAsync();
        var problemsResponse = _mapper.Map<List<ProblemResponseDto>>(problems, opts =>
        {
            opts.Items["lang"] = dto.Lang;
        });

        return new GetRanklistRowsResponseWithPropsDto(
            problemsResponse,
            rowsResponse,
            typeof(GetRanklistRowsResponseDto).GetProperties().Select(p => p.Name).ToArray());
    }
}
