using Etrx.Application.Dtos.Problems;

namespace Etrx.Application.Interfaces;

public interface IProblemPreferencesService
{
    Task<ProblemPreferencesResponseDto> GetAsync(Guid userId, CancellationToken cancellationToken, bool ladder = false);
    Task SaveAsync(Guid userId, string? currentHandle, SaveProblemPreferencesRequestDto request, CancellationToken cancellationToken, bool ladder = false);
}
