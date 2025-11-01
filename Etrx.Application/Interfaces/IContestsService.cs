using Etrx.Domain.Dtos.Contests;

namespace Etrx.Application.Interfaces;

public interface IContestsService
{
    Task<List<ContestResponseDto>> GetAllContestsAsync(string lang);
    Task<ContestResponseDto?> GetContestByIdAsync(int contestId, string lang);
    Task<ContestWithPropsResponseDto> GetContestsByPageWithSortAsync(GetSortContestRequestDto dto);
}