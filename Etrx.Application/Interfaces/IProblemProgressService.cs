using Etrx.Application.Dtos.Problems;

namespace Etrx.Application.Interfaces;

public interface IProblemProgressService
{
    Task EnrichAsync(Guid userId, IEnumerable<ProblemResponseDto> items, CancellationToken ct);
    Task<bool> SetDeferredAsync(Guid userId, int contestId, string index, bool deferred, CancellationToken ct);
    Task<ProblemSyncResultDto> SyncAsync(Guid userId, string handle, CancellationToken ct);
}
