using Etrx.Application.Dtos.Problems;

namespace Etrx.Application.Interfaces;

public interface IProblemPreferencesService
{
    Task<ProblemPreferencesResponseDto> GetAsync(Guid userId, CancellationToken ct);
    Task<PreferenceSaveStatus> SaveAsync(Guid userId, string? currentHandle, SaveProblemPreferencesRequestDto request, CancellationToken ct);
}
