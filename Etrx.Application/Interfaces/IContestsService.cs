using Etrx.Core.Contracts.Contests;
using Etrx.Domain.Models;

namespace Etrx.Application.Interfaces;

public interface IContestsService
{
    Task<List<Contest>> GetAllContestsAsync();
    Task<ContestResponseDto?> GetContestByIdAsync(int contestId);
    ContestWithPropsResponseDto GetContestsByPageWithSortAsync(GetSortContestRequestDto dto);
}