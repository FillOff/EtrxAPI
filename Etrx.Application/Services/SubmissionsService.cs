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
        var range = ProtocolDateRange.ToUnix(dto.FYear, dto.FMonth, dto.FDay,
            dto.TYear, dto.TMonth, dto.TDay);
        var result = await _unitOfWork.Submissions.GetUsersProtocolAsync(
            range.From,
            range.Until - 1,
            dto.ContestId);

        return result;
    }

    public async Task<List<DivisionResultResponseDto>> GetDivisionResultsAsync(
        GetDivisionResultsRequestDto dto, CancellationToken cancellationToken = default)
    {
        var range = ProtocolDateRange.ToUnix(dto.FYear, dto.FMonth, dto.FDay,
            dto.TYear, dto.TMonth, dto.TDay);
        var facts = await _unitOfWork.Submissions.GetResultFactsAsync(
            range.From, range.Until, dto.ContestId, cancellationToken);
        return ProtocolResultsCalculator.Results(facts, dto.Division, dto.ParticipantType);
    }


    public async Task<List<ProtocolUserMatrixResponseDto>> GetResultsMatrixAsync(
        GetDivisionResultsRequestDto dto, CancellationToken cancellationToken = default)
    {
        var range = ProtocolDateRange.ToUnix(dto.FYear, dto.FMonth, dto.FDay,
            dto.TYear, dto.TMonth, dto.TDay);
        var facts = await _unitOfWork.Submissions.GetResultFactsAsync(
            range.From, range.Until, dto.ContestId, cancellationToken);
        return ProtocolResultsCalculator.UserMatrix(facts);
    }

    public async Task<List<ProtocolUserTypeMatrixResponseDto>> GetResultsByTypeAsync(
        GetDivisionResultsRequestDto dto, CancellationToken cancellationToken = default)
    {
        var range = ProtocolDateRange.ToUnix(dto.FYear, dto.FMonth, dto.FDay,
            dto.TYear, dto.TMonth, dto.TDay);
        var facts = await _unitOfWork.Submissions.GetResultFactsAsync(
            range.From, range.Until, dto.ContestId, cancellationToken);
        return ProtocolResultsCalculator.UserTypeMatrix(facts, dto.Division);
    }

    public async Task<List<ProtocolTaskDetailResponseDto>> GetResultsTasksAsync(
        string handle, GetDivisionResultsRequestDto dto, CancellationToken cancellationToken = default)
    {
        var range = ProtocolDateRange.ToUnix(dto.FYear, dto.FMonth, dto.FDay,
            dto.TYear, dto.TMonth, dto.TDay);
        var facts = await _unitOfWork.Submissions.GetResultFactsAsync(
            range.From, range.Until, dto.ContestId, cancellationToken);
        return ProtocolResultsCalculator.Tasks(facts, handle, dto.Division, dto.ParticipantType);
    }

    public async Task<List<ProtocolSummaryResponseDto>> GetResultsSummaryAsync(
        GetDivisionResultsRequestDto dto, CancellationToken cancellationToken = default)
    {
        var range = ProtocolDateRange.ToUnix(dto.FYear, dto.FMonth, dto.FDay,
            dto.TYear, dto.TMonth, dto.TDay);
        var facts = await _unitOfWork.Submissions.GetResultFactsAsync(
            range.From, range.Until, dto.ContestId, cancellationToken);
        return ProtocolResultsCalculator.Summary(facts);
    }

    public async Task<List<GetUserProtocolResponseDto>> GetUserProtocolAsync(string handle, GetUserContestProtocolRequestDto dto)
    {
        _ = await _unitOfWork.Users.GetByHandleAsync(handle)
            ?? throw new NotFoundException($"User {handle} not found");

        var range = ProtocolDateRange.ToUnix(dto.FYear, dto.FMonth, dto.FDay,
            dto.TYear, dto.TMonth, dto.TDay);
        var result = await _unitOfWork.Submissions.GetUserProtocolAsync(
            handle,
            range.From,
            range.Until - 1);

        return result;
    }

    public async Task<List<GetUserContestProtocolResponseDto>> GetUserContestProtocolAsync(string handle, int contestId, GetUserContestProtocolRequestDto dto)
    {
        _ = await _unitOfWork.Users.GetByHandleAsync(handle)
            ?? throw new NotFoundException($"User {handle} not found");

        _ = await _unitOfWork.Contests.GetByContestIdAsync(contestId)
            ?? throw new NotFoundException($"Contest {contestId} not found");

        var range = ProtocolDateRange.ToUnix(dto.FYear, dto.FMonth, dto.FDay,
            dto.TYear, dto.TMonth, dto.TDay);
        var submissions = await _unitOfWork.Submissions.GetUserContestProtocolAsync(
            handle, contestId,
            range.From,
            range.Until - 1);
        var response = _mapper.Map<List<GetUserContestProtocolResponseDto>>(submissions);

        return response;
    }
}
