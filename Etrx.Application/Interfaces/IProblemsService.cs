using Etrx.Core.Contracts.Problems;
using Etrx.Domain.Models;

namespace Etrx.Application.Interfaces;

public interface IProblemsService
{
    Task<List<ProblemResponseDto>> GetAllProblemsAsync();
    Task<ProblemResponseDto?> GetProblemByContestIdAndIndexAsync(
        int contestId,
        string index);
    Task<List<ProblemResponseDto>> GetProblemsByContestIdAsync(int contestId);
    ProblemWithPropsResponseDto GetProblemsByPageWithSortAndFilterTagsAsync(GetSortProblemRequestDto dto);
    Task<List<string>> GetAllTagsAsync();
    Task<List<string>> GetAllIndexesAsync();
    Task<List<string>> GetProblemsIndexesByContestIdAsync(int contestId);
}