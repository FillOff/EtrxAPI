using Etrx.Application.Dtos.Problems;

namespace Etrx.Application.Interfaces;

public interface IProblemProgressService
{
    Task EnrichAsync(Guid userId, IEnumerable<ProblemResponseDto> items, CancellationToken cancellationToken);
    Task SetDeferredAsync(Guid userId, string? currentHandle, int contestId, string index, SetDeferredRequestDto request, CancellationToken cancellationToken);
    Task<ProblemSyncResponseDto> SyncAsync(Guid userId, string handle, CancellationToken cancellationToken);
}
