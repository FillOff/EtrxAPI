using Etrx.Application.Dtos.Problems;
using Etrx.Application.Interfaces;
using Etrx.Application.Repositories;
using Microsoft.Extensions.Logging;

namespace Etrx.Application.Services;

public sealed class ProblemProgressService(IProblemProgressRepository repository,
    ICodeforcesProgressClient codeforces, ILogger<ProblemProgressService> logger) : IProblemProgressService
{
    private const string SyncFailedMessage = "Codeforces is temporarily unavailable. Saved results are shown.";
    private const long TicksPerMicrosecond = 10;

    public async Task EnrichAsync(Guid userId, IEnumerable<ProblemResponseDto> items, CancellationToken ct)
    {
        var problems = items.ToList();
        var contestIds = problems.Select(p => p.ContestId).Distinct().ToArray();
        if (contestIds.Length == 0)
        {
            return;
        }

        var rows = await repository.GetProgressAsync(userId, contestIds, ct);
        var states = rows.ToDictionary(p => (p.ContestId, p.Index));
        foreach (var problem in problems)
        {
            if (states.TryGetValue((problem.ContestId, problem.Index), out var state))
            {
                problem.Status = state.Status;
                problem.IsDeferred = state.IsDeferred;
            }
        }
    }

    public async Task<bool> SetDeferredAsync(Guid userId, int contestId, string index, bool deferred, CancellationToken ct)
    {
        if (!await repository.ProblemExistsAsync(contestId, index, ct))
        {
            return false;
        }

        await repository.SetDeferredAsync(userId, contestId, index, deferred, ct);
        return true;
    }

    public async Task<ProblemSyncResultDto> SyncAsync(Guid userId, string handle, CancellationToken ct)
    {
        var ticks = DateTimeOffset.UtcNow.Ticks;
        var attempt = new DateTimeOffset(ticks - ticks % TicksPerMicrosecond, TimeSpan.Zero);
        if (!await repository.TryStartSyncAsync(userId, attempt, ct))
        {
            var previous = await repository.GetSyncAsync(userId, ct);
            var success = previous?.LastSuccessAt >= previous?.LastAttemptAt;
            return new ProblemSyncResultDto(success, previous?.LastSuccessAt, success ? null : SyncFailedMessage, true);
        }

        try
        {
            var history = await codeforces.GetHistoryAsync(handle, ct);
            var successAt = await repository.MergeProgressAsync(userId, attempt, history, ct);
            return new ProblemSyncResultDto(true, successAt, null);
        }
        catch (Exception exception) when (exception is HttpRequestException or System.Text.Json.JsonException ||
            exception is OperationCanceledException && !ct.IsCancellationRequested)
        {
            logger.LogWarning(exception, "Could not synchronize problem progress for user {UserId}", userId);
            var previous = await repository.GetSyncAsync(userId, ct);
            return new ProblemSyncResultDto(false, previous?.LastSuccessAt, SyncFailedMessage);
        }
    }
}
