using Etrx.Application.Dtos.Problems;
using Etrx.Domain.Models;

namespace Etrx.Application.Repositories;

public interface IProblemProgressRepository
{
    Task<List<ProblemProgressRowDto>> GetProgressAsync(Guid userId, int[] contestIds, CancellationToken cancellationToken);
    Task<bool> ProblemExistsAsync(int contestId, string index, CancellationToken cancellationToken);
    Task SetDeferredAsync(Guid userId, int contestId, string index, bool deferred, CancellationToken cancellationToken);
    Task<bool> TryStartSyncAsync(Guid userId, DateTimeOffset now, CancellationToken cancellationToken);
    Task<UserProblemSync?> GetSyncAsync(Guid userId, CancellationToken cancellationToken);
    Task<DateTimeOffset> MergeProgressAsync(Guid userId, DateTimeOffset attempt,
        IReadOnlyDictionary<(int ContestId, string Index), ProgressFacts> history, CancellationToken cancellationToken);
}
