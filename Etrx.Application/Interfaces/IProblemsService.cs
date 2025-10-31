using Etrx.Core.Contracts.Problems;

namespace Etrx.Application.Interfaces;

public interface IProblemsService
{
    Task<List<ProblemResponseDto>> GetAllProblemsAsync(string lang);
    Task<ProblemResponseDto?> GetProblemByContestIdAndIndexAsync(
        int contestId,
        string index,
        string lang);
    Task<List<ProblemResponseDto>> GetProblemsByContestIdAsync(int contestId, string lang);
    ProblemWithPropsResponseDto GetProblemsByPageWithSortAndFilterTagsAsync(GetSortProblemRequestDto dto);
    Task<List<string>> GetAllTagsAsync(GetSortProblemRequestDto dto);
    Task<List<string>> GetAllIndexesAsync();
    Task<List<string>> GetProblemsIndexesByContestIdAsync(int contestId);
} 