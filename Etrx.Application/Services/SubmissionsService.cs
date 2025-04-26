using Etrx.Domain.Models;
using System.Linq.Dynamic.Core;
using Etrx.Persistence.Interfaces;
using Etrx.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Etrx.Core.Contracts.Submissions;
using AutoMapper;
using Etrx.Domain.Contracts.Submissions;

namespace Etrx.Application.Services;

public class SubmissionsService : ISubmissionsService
{
    private readonly ISubmissionsRepository _submissionsRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IProblemsRepository _problemsRepository;
    private readonly IMapper _mapper;

    public SubmissionsService(
        ISubmissionsRepository submissionsRepository,
        IUsersRepository usersRepository,
        IProblemsRepository problemsRepository,
        IMapper mapper)
    {
        _submissionsRepository = submissionsRepository;
        _usersRepository = usersRepository;
        _problemsRepository = problemsRepository;
        _mapper = mapper;
    }

    public async Task<List<SubmissionsResponseDto>> GetAllSubmissionsAsync()
    {
        var submissions =  await _submissionsRepository.GetAll()
            .ToListAsync();
        var response = _mapper.Map<List<SubmissionsResponseDto>>(submissions);

        return response;
    }

    public async Task<List<SubmissionsResponseDto>> GetSubmissionsByContestIdAsync(int contestId)
    {
        var submissions = await _submissionsRepository.GetByContestId(contestId).ToListAsync();
        var response = _mapper.Map<List<SubmissionsResponseDto>>(submissions);

        return response;
    }

    public async Task<List<string>> GetUserParticipantTypesAsync(string handle)
    {
        return await _submissionsRepository.GetUserParticipantTypes(handle).ToListAsync();
    }

    public async Task<SubmissionsWithProblemIndexesResponseDto> GetSubmissionsWithSortAsync(
        int contestId, 
        GetSubmissionRequestDto dto)
    {
        if (string.IsNullOrEmpty(dto.SortField) ||
            !typeof(SubmissionsResponseDto).GetProperties().Any(p => p.Name.Equals(dto.SortField, StringComparison.InvariantCultureIgnoreCase)))
        {
            throw new Exception($"Invalid field: SortField");
        }

        string order = dto.SortOrder == true ? "asc" : "desc";

        var submissions = _submissionsRepository.GetByContestId(contestId);

        var handles = submissions
            .Select(s => s.Handle)
            .Distinct()
            .ToArray();

        List<User> users = [];
        foreach ( var handle in handles)
        {
            users.Add((await _usersRepository.GetByKey(handle))!);
        }

        List<SubmissionsResponseDto> submissionsResponses = [];

        List<string>? indexes;
        if (dto.AllIndexes)
        {
            indexes = await _problemsRepository
                .GetIndexesByContestId(contestId)
                .ToListAsync();
        }
        else
        {
            indexes = submissions
                .Select(s => s.Index)
                .Distinct()
                .ToList();
        }

        foreach (var handle in handles)
        {
            var userSubmissions = submissions.Where(s => s.Handle == handle).ToList();
            List<string> types = await GetUserParticipantTypesAsync(handle);
            foreach (var type in types)
            {
                var typeSubmissions = userSubmissions.Where(s => s.ParticipantType == type).ToList();

                var (solvedCount, tries) = GetTriesAndSolvedCountByHandleAsync(typeSubmissions, indexes);
                var user = users.FirstOrDefault(u => u!.Handle.Equals(handle, StringComparison.CurrentCultureIgnoreCase))!;

                var submissionResponse = new SubmissionsResponseDto(handle, user.FirstName, user.LastName, user.City, user.Organization,
                    user.Grade, solvedCount, type, tries);

                submissionsResponses.Add(submissionResponse);
            }
        }

        if (dto.FilterByParticipantType != "ALL")
        {
            submissionsResponses = submissionsResponses
                .Where(s => s.ParticipantType == dto.FilterByParticipantType)
                .ToList();
        }

        submissionsResponses = submissionsResponses
            .AsQueryable()
            .OrderBy($"{dto.SortField} {order}")
            .ToList();

        var response = new SubmissionsWithProblemIndexesResponseDto(
            Submissions: submissionsResponses,
            ProblemIndexes: indexes,
            Properties: typeof(SubmissionsResponseDto).GetProperties().Select(p => p.Name).ToList());

        return response;
    }

    public async Task<GetSubmissionsWithPropsProtocolResponseDto> GetProtocolAsync(GetSubmissionsProtocolRequestDto dto)
    {
        var unixFromDateTime = (new DateTime(dto.FYear, dto.FMonth, dto.FDay).AddHours(3) - DateTimeOffset.UnixEpoch).TotalSeconds;
        var unixToDateTime = (new DateTime(dto.TYear, dto.TMonth, dto.TDay).AddHours(20).AddMinutes(59) - DateTimeOffset.UnixEpoch).TotalSeconds;

        if (dto.Page <= 0)
        {
            throw new Exception($"Invalid field: Page");
        }

        if (dto.PageSize <= 0)
        {
            throw new Exception($"Invalid field: PageSize");
        }

        var submissions = _submissionsRepository.GetAll()
            .Where(s => s.CreationTimeSeconds <= unixToDateTime && s.CreationTimeSeconds >= unixFromDateTime);

        if (dto.ContestId != null)
        {
            submissions = submissions.Where(s => s.ContestId == dto.ContestId);
        }

        int pageCount = submissions.Count() % dto.PageSize == 0
            ? submissions.Count() / dto.PageSize
            : submissions.Count() / dto.PageSize + 1;

        submissions = submissions
            .OrderBy("CreationTimeSeconds desc")
            .Skip((dto.Page - 1) * dto.PageSize)
            .Take(dto.PageSize);

        return new GetSubmissionsWithPropsProtocolResponseDto(
            Submissions: _mapper.Map<List<GetSubmissionsProtocolResponseDto>>(await submissions.ToListAsync()),
            Properties: typeof(GetSubmissionsProtocolResponseDto).GetProperties().Select(p => p.Name).ToArray(),
            PageCount: pageCount);
    }

    public async Task<List<GetGroupSubmissionsProtocolResponseDto>> GetGroupProtocolAsync(GetGroupSubmissionsProtocolRequestDto dto)
    {
        var unixFromDateTime = (new DateTime(dto.FYear, dto.FMonth, dto.FDay).AddHours(3) - DateTimeOffset.UnixEpoch).TotalSeconds;
        var unixToDateTime = (new DateTime(dto.TYear, dto.TMonth, dto.TDay).AddHours(20).AddMinutes(59) - DateTimeOffset.UnixEpoch).TotalSeconds;

        var submissions = _submissionsRepository.GetAll()
            .Where(s => s.CreationTimeSeconds >= unixFromDateTime && s.CreationTimeSeconds <= unixToDateTime);

        if (dto.ContestId != null)
        {
            submissions = submissions.Where(s => s.ContestId == dto.ContestId);
        }

        var groupedSubmissions = await submissions
            .Where(s => s.Verdict == "OK")
            .GroupBy(s => s.Handle)
            .Select(g => new GetGroupSubmissionsProtocolResponseDto
            {
                UserName = $"{g.First().User.LastName} {g.First().User.FirstName}",
                ContestId = g.First().ContestId,
                SolvedCount = g.Select(s => s.Index).Distinct().Count()
            })
            .OrderByDescending(dto => dto.SolvedCount)
            .ToListAsync();

        return groupedSubmissions;
    }

    private (int SolvedCount, List<int> Tries) GetTriesAndSolvedCountByHandleAsync(
        List<Submission> userSubmissions,
        List<string> indexes)
    {
        int solvedCount = 0;
        List<int> tries = indexes
            .Select(index =>
            {
                var submissions = userSubmissions.Where(s => s.Index == index).ToList();
                int tryCount = submissions.Count;
                bool isSolved = submissions.Any(s => s.Verdict == "OK");

                if (isSolved) solvedCount++;

                return isSolved ? tryCount : -tryCount;
            })
            .ToList();

        return (solvedCount, tries);
    }
}
