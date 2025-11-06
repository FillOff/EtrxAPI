using Etrx.Domain.Dtos.Submissions;

namespace Etrx.Application.Interfaces;

public interface ISubmissionsService
{
    Task<List<SubmissionsResponseDto>> GetAllSubmissionsAsync();
    Task<List<SubmissionsResponseDto>> GetSubmissionsByContestIdAsync(int contestId);
    Task<List<string>> GetUserParticipantTypesAsync(string handle);
    Task<GetGroupSubmissionsProtocolWithPropsResponseDto> GetGroupProtocolAsync(GetGroupSubmissionsProtocolRequestDto dto);
    Task<List<GetUserContestProtocolResponseDto>> GetUserContestProtocolAsync(string handle, int contestId, GetUserContestProtocolRequestDto dto);
}