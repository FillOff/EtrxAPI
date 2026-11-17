using Etrx.Application.Dtos.Problems;
using Etrx.Application.Constants;
using Etrx.Application.Exceptions;
using Etrx.Application.Exceptions.BadRequest;
using Etrx.Application.Exceptions.NotFound;
using Etrx.Application.Interfaces;
using Etrx.Application.Repositories;
using Microsoft.Extensions.Logging;

namespace Etrx.Application.Services;

public sealed class ProblemProgressService : IProblemProgressService
{
    private readonly IProblemProgressRepository _repository;
    private readonly ICodeforcesProgressClient _codeforces;
    private readonly ILogger<ProblemProgressService> _logger;

    private const long TicksPerMicrosecond = 10;

    public ProblemProgressService(IProblemProgressRepository repository, ICodeforcesProgressClient codeforces, ILogger<ProblemProgressService> logger)
    {
        _repository = repository;
        _codeforces = codeforces;
        _logger = logger;
    }

    public async Task EnrichAsync(Guid userId, IEnumerable<ProblemResponseDto> items, CancellationToken cancellationToken)
    {
        var problems = items.ToList();
        var contestIds = problems.Select(p => p.ContestId).Distinct().ToArray();
        if (contestIds.Length == 0)
        {
            return;
        }

        var rows = await _repository.GetProgressAsync(userId, contestIds, cancellationToken);
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

    public async Task SetDeferredAsync(Guid userId, string? currentHandle, int contestId, string index,
        SetDeferredRequestDto request, CancellationToken cancellationToken)
    {
        if (contestId <= 0 || string.IsNullOrWhiteSpace(index) || index.Length > RequestLimits.MaxProblemIndexLength)
        {
            throw new BadRequestException("Invalid problem identifier.");
        }

        if (string.IsNullOrWhiteSpace(request.ExpectedHandle) || request.ExpectedHandle.Length > RequestLimits.MaxHandleLength)
        {
            throw new BadRequestException("Invalid expected handle.");
        }

        if (!string.Equals(currentHandle, request.ExpectedHandle, StringComparison.OrdinalIgnoreCase))
        {
            throw new ConflictException("The account changed. The problem was not updated.");
        }

        if (!await _repository.ProblemExistsAsync(contestId, index, cancellationToken))
        {
            throw new NotFoundException("Problem not found.");
        }

        await _repository.SetDeferredAsync(userId, contestId, index, request.Deferred, cancellationToken);
    }

    public async Task<ProblemSyncResponseDto> SyncAsync(Guid userId, string handle, CancellationToken cancellationToken)
    {
        var ticks = DateTimeOffset.UtcNow.Ticks;
        var attempt = new DateTimeOffset(ticks - ticks % TicksPerMicrosecond, TimeSpan.Zero);
        if (!await _repository.TryStartSyncAsync(userId, attempt, cancellationToken))
        {
            var previous = await _repository.GetSyncAsync(userId, cancellationToken);
            var success = previous?.LastSuccessAt >= previous?.LastAttemptAt;
            return new ProblemSyncResponseDto(success, previous?.LastSuccessAt, success ? null : "Codeforces is temporarily unavailable. Saved results are shown.", true);
        }

        try
        {
            var history = await _codeforces.GetHistoryAsync(handle, cancellationToken);
            var successAt = await _repository.MergeProgressAsync(userId, attempt, history, cancellationToken);
            return new ProblemSyncResponseDto(true, successAt, null);
        }
        catch (Exception exception) when (exception is HttpRequestException or System.Text.Json.JsonException ||
            exception is OperationCanceledException && !cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(exception, "Could not synchronize problem progress for user {UserId}", userId);
            var previous = await _repository.GetSyncAsync(userId, cancellationToken);
            return new ProblemSyncResponseDto(false, previous?.LastSuccessAt, "Codeforces is temporarily unavailable. Saved results are shown.");
        }
    }
}
