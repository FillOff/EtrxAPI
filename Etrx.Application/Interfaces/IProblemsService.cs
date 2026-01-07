using Etrx.Application.Dtos.Problems;

namespace Etrx.Application.Interfaces;

public interface IProblemsService
{
    Task<List<ProblemResponseDto>> GetAllProblemsAsync(string lang);
    Task<ProblemResponseDto?> GetProblemByContestIdAndIndexAsync(int contestId, string index, string lang);
    Task<List<ProblemResponseDto>> GetProblemsByContestIdAsync(int contestId, string lang);
    Task<ProblemWithPropsResponseDto> GetProblemsByPageWithSortAndFilterAsync(GetSortProblemRequestDto dto);
    Task<List<string>> GetProblemsIndexesByContestIdAsync(int contestId);
    Task<GetProblemFiltersResponseDto> GetProblemFiltersAsync(GetSortProblemRequestDto dto);
}