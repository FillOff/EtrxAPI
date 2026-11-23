using Etrx.Application.Dtos.Submissions;

namespace Etrx.Application.Interfaces;

public interface ISubmissionsService
{
    Task<List<GetUsersProtocolsResponseDto>> GetUsersProtocolAsync(GetUsersProtocolRequestDto dto);
    Task<List<DivisionResultResponseDto>> GetDivisionResultsAsync(GetDivisionResultsRequestDto dto, CancellationToken cancellationToken = default);
    Task<List<ProtocolUserMatrixResponseDto>> GetResultsMatrixAsync(GetDivisionResultsRequestDto dto, CancellationToken cancellationToken = default);
    Task<List<ProtocolUserTypeMatrixResponseDto>> GetResultsByTypeAsync(GetDivisionResultsRequestDto dto, CancellationToken cancellationToken = default);
    Task<List<ProtocolTaskDetailResponseDto>> GetResultsTasksAsync(string handle, GetDivisionResultsRequestDto dto, CancellationToken cancellationToken = default);
    Task<List<ProtocolSummaryResponseDto>> GetResultsSummaryAsync(GetDivisionResultsRequestDto dto, CancellationToken cancellationToken = default);
    Task<List<GetUserProtocolResponseDto>> GetUserProtocolAsync(string handle, GetUserContestProtocolRequestDto dto);
    Task<List<GetUserContestProtocolResponseDto>> GetUserContestProtocolAsync(string handle, int contestId, GetUserContestProtocolRequestDto dto);
}
