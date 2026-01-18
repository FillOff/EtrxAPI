using AutoMapper;
using Etrx.Application.Dtos.Submissions;
using Etrx.Application.Exceptions;
using Etrx.Application.Interfaces;
using Etrx.Application.Queries;
using Etrx.Application.Queries.Common;
using Etrx.Application.Repositories.UnitOfWork;

namespace Etrx.Application.Services;

public class SubmissionsService : ISubmissionsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SubmissionsService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<GetGroupSubmissionsProtocolWithPropsResponseDto> GetGroupProtocolAsync(GetGroupSubmissionsProtocolRequestDto dto)
    {
        var queryParams = new GroupProtocolQueryParameters(
            new SortingQueryParameters(dto.SortField, dto.SortOrder),
            (long)(new DateTime(dto.FYear, dto.FMonth, dto.FDay).AddHours(3) - DateTimeOffset.UnixEpoch).TotalSeconds,
            (long)(new DateTime(dto.TYear, dto.TMonth, dto.TDay).AddHours(20).AddMinutes(59) - DateTimeOffset.UnixEpoch).TotalSeconds,
            dto.ContestId);

        return new GetGroupSubmissionsProtocolWithPropsResponseDto(
            Submissions: await _unitOfWork.Submissions.GetGroupProtocolWithSortAsync(queryParams),
            Properties: typeof(GetGroupSubmissionsProtocolResponseDto).GetProperties().Select(p => p.Name).ToList());
    }

    public async Task<List<GetUserContestProtocolResponseDto>> GetUserContestProtocolAsync(string handle, int contestId, GetUserContestProtocolRequestDto dto)
    {
        _ = await _unitOfWork.Users.GetByHandleAsync(handle)
            ?? throw new NotFoundException($"User {handle} not found");

        _ = await _unitOfWork.Contests.GetByContestIdAsync(contestId)
            ?? throw new NotFoundException($"Contest {contestId} not found");

        var queryParams = new HandleContestProtocolQueryParameters(
            handle, contestId,
            (long)(new DateTime(dto.FYear, dto.FMonth, dto.FDay).AddHours(3) - DateTimeOffset.UnixEpoch).TotalSeconds,
            (long)(new DateTime(dto.TYear, dto.TMonth, dto.TDay).AddHours(20).AddMinutes(59) - DateTimeOffset.UnixEpoch).TotalSeconds);

        var submissions = await _unitOfWork.Submissions.GetByHandleAndContestIdAsync(queryParams);
        var response = _mapper.Map<List<GetUserContestProtocolResponseDto>>(submissions);

        return response;
    }
}