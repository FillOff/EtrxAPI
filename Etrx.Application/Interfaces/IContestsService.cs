using Etrx.Core.Contracts.Contests;
using Etrx.Domain.Models;

namespace Etrx.Application.Interfaces;

public interface IContestsService
{
    Task<List<ContestResponseDto>> GetAllContestsAsync(string lang);
    Task<ContestResponseDto?> GetContestByIdAsync(int contestId, string lang);
    ContestWithPropsResponseDto GetContestsByPageWithSortAsync(GetSortContestRequestDto dto);
}