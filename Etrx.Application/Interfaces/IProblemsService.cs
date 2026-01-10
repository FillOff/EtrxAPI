using Etrx.Application.Dtos.Problems;

namespace Etrx.Application.Interfaces;

public interface IProblemsService
{
    Task<List<ProblemResponseDto>> GetProblemsByContestIdAsync(int contestId, string lang);
    Task<ProblemWithPropsResponseDto> GetProblemsByPageWithSortAndFilterAsync(GetSortProblemRequestDto dto);
    Task<GetProblemFiltersResponseDto> GetProblemFiltersAsync(GetSortProblemRequestDto dto);
}