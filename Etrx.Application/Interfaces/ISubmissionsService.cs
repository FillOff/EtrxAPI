using Etrx.Application.Dtos.Submissions;

namespace Etrx.Application.Interfaces;

public interface ISubmissionsService
{
    Task<List<GetUsersProtocolsResponseDto>> GetUsersProtocolAsync(GetUsersProtocolRequestDto dto);
    Task<List<GetUserProtocolResponseDto>> GetUserProtocolAsync(string handle, GetUserContestProtocolRequestDto dto);
    Task<List<GetUserContestProtocolResponseDto>> GetUserContestProtocolAsync(string handle, int contestId, GetUserContestProtocolRequestDto dto);
}