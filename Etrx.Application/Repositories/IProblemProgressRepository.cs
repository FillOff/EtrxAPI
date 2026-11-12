using Etrx.Application.Dtos.Problems;
using Etrx.Domain.Models;

namespace Etrx.Application.Repositories;

public interface IProblemProgressRepository
{
    Task<List<ProblemProgressRowDto>> GetProgressAsync(Guid userId, int[] contestIds, CancellationToken ct);
    Task<bool> ProblemExistsAsync(int contestId, string index, CancellationToken ct);
    Task SetDeferredAsync(Guid userId, int contestId, string index, bool deferred, CancellationToken ct);
    Task<bool> TryStartSyncAsync(Guid userId, DateTimeOffset now, CancellationToken ct);
    Task<UserProblemSync?> GetSyncAsync(Guid userId, CancellationToken ct);
    Task<DateTimeOffset> MergeProgressAsync(Guid userId, DateTimeOffset attempt,
        IReadOnlyDictionary<(int ContestId, string Index), ProgressFacts> history, CancellationToken ct);
}
