using Etrx.Core.Contracts.Submissions;
using Etrx.Domain.Models;

namespace Etrx.Application.Interfaces;

public interface ISubmissionsService
{
    Task<List<SubmissionsResponseDto>> GetAllSubmissionsAsync();
    Task<List<SubmissionsResponseDto>> GetSubmissionsByContestIdAsync(int contestId);
    Task<List<string>> GetUserParticipantTypesAsync(string handle);
    Task<SubmissionsWithProblemIndexesResponseDto> GetSubmissionsWithSortAsync(
        int contestId,
        GetSubmissionRequestDto dto);
}