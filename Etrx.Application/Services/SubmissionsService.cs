using Etrx.Application.Interfaces;
using AutoMapper;
using Etrx.Domain.Interfaces;
using Etrx.Domain.Dtos.Submissions;
using Etrx.Domain.Queries;
using Etrx.Domain.Queries.Common;

namespace Etrx.Application.Services;

public class SubmissionsService : ISubmissionsService
{
    private readonly ISubmissionsRepository _submissionsRepository;
    private readonly IMapper _mapper;

    public SubmissionsService(
        ISubmissionsRepository submissionsRepository,
        IMapper mapper)
    {
        _submissionsRepository = submissionsRepository;
        _mapper = mapper;
    }

    public async Task<List<SubmissionsResponseDto>> GetAllSubmissionsAsync()
    {
        var submissions =  await _submissionsRepository.GetAllAsync();
        var response = _mapper.Map<List<SubmissionsResponseDto>>(submissions);

        return response;
    }

    public async Task<List<SubmissionsResponseDto>> GetSubmissionsByContestIdAsync(int contestId)
    {
        var submissions = await _submissionsRepository.GetByContestIdAsync(contestId);
        var response = _mapper.Map<List<SubmissionsResponseDto>>(submissions);

        return response;
    }

    public async Task<List<string>> GetUserParticipantTypesAsync(string handle)
    {
        return await _submissionsRepository.GetUserParticipantTypesAsync(handle);
    }

    public async Task<GetGroupSubmissionsProtocolWithPropsResponseDto> GetGroupProtocolAsync(GetGroupSubmissionsProtocolRequestDto dto)
    {
        var queryParams = new GroupProtocolQueryParameters(
            new SortingQueryParameters(dto.SortField, dto.SortOrder),
            (long)(new DateTime(dto.FYear, dto.FMonth, dto.FDay).AddHours(3) - DateTimeOffset.UnixEpoch).TotalSeconds,
            (long)(new DateTime(dto.TYear, dto.TMonth, dto.TDay).AddHours(20).AddMinutes(59) - DateTimeOffset.UnixEpoch).TotalSeconds,
            dto.ContestId);

        return new GetGroupSubmissionsProtocolWithPropsResponseDto(
            Submissions: await _submissionsRepository.GetGroupProtocolWithSortAsync(queryParams),
            Properties: typeof(GetGroupSubmissionsProtocolResponseDto).GetProperties().Select(p => p.Name).ToList());
    }
}
