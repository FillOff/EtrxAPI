using AutoMapper;
using Etrx.Application.Dtos.Submissions;
using Etrx.Application.Exceptions.NotFound;
using Etrx.Application.Interfaces;
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

    public async Task<List<GetUsersProtocolsResponseDto>> GetUsersProtocolAsync(GetUsersProtocolRequestDto dto)
    {
        var result = await _unitOfWork.Submissions.GetUsersProtocolAsync(
            (long)(new DateTime(dto.FYear, dto.FMonth, dto.FDay).AddHours(3) - DateTimeOffset.UnixEpoch).TotalSeconds,
            (long)(new DateTime(dto.TYear, dto.TMonth, dto.TDay).AddHours(20).AddMinutes(59) - DateTimeOffset.UnixEpoch).TotalSeconds,
            dto.ContestId);

        return result;
    }

    public async Task<List<GetUserProtocolResponseDto>> GetUserProtocolAsync(string handle, GetUserContestProtocolRequestDto dto)
    {
        _ = await _unitOfWork.Users.GetByHandleAsync(handle)
            ?? throw new NotFoundException($"User {handle} not found");

        var result = await _unitOfWork.Submissions.GetUserProtocolAsync(
            handle,
            (long)(new DateTime(dto.FYear, dto.FMonth, dto.FDay).AddHours(3) - DateTimeOffset.UnixEpoch).TotalSeconds,
            (long)(new DateTime(dto.TYear, dto.TMonth, dto.TDay).AddHours(20).AddMinutes(59) - DateTimeOffset.UnixEpoch).TotalSeconds);

        return result;
    }

    public async Task<List<GetUserContestProtocolResponseDto>> GetUserContestProtocolAsync(string handle, int contestId, GetUserContestProtocolRequestDto dto)
    {
        _ = await _unitOfWork.Users.GetByHandleAsync(handle)
            ?? throw new NotFoundException($"User {handle} not found");

        _ = await _unitOfWork.Contests.GetByContestIdAsync(contestId)
            ?? throw new NotFoundException($"Contest {contestId} not found");

        var submissions = await _unitOfWork.Submissions.GetUserContestProtocolAsync(
            handle, contestId,
            (long)(new DateTime(dto.FYear, dto.FMonth, dto.FDay).AddHours(3) - DateTimeOffset.UnixEpoch).TotalSeconds,
            (long)(new DateTime(dto.TYear, dto.TMonth, dto.TDay).AddHours(20).AddMinutes(59) - DateTimeOffset.UnixEpoch).TotalSeconds);
        var response = _mapper.Map<List<GetUserContestProtocolResponseDto>>(submissions);

        return response;
    }
}