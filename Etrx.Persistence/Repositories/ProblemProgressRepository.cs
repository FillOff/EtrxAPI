using Etrx.Application.Dtos.Problems;
using Etrx.Application.Repositories;
using Etrx.Domain.Models;
using Etrx.Persistence.Databases;
using Etrx.Persistence.Queries;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Etrx.Persistence.Repositories;

public sealed class ProblemProgressRepository(EtrxDbContext db) : IProblemProgressRepository
{
    private const int ProgressBatchSize = 500;
    private static readonly TimeSpan SyncInterval = TimeSpan.FromSeconds(60);

    public async Task<List<ProblemProgressRowDto>> GetProgressAsync(Guid userId, int[] contestIds, CancellationToken ct)
    {
        var rows = await ProblemProgressQuery.ForUser(db, userId)
            .Where(p => contestIds.Contains(p.ContestId)).ToListAsync(ct);
        return rows.Select(p => new ProblemProgressRowDto(p.ContestId, p.Index, p.Status, p.IsDeferred)).ToList();
    }

    public Task<bool> ProblemExistsAsync(int contestId, string index, CancellationToken ct) =>
        db.Problems.AnyAsync(p => p.ContestId == contestId && p.Index == index, ct);

    public async Task SetDeferredAsync(Guid userId, int contestId, string index, bool deferred, CancellationToken ct)
    {
        var query = db.UserProblemProgress.Where(p => p.UserId == userId && p.ContestId == contestId && p.Index == index);
        if (await query.ExecuteUpdateAsync(set => set.SetProperty(p => p.IsDeferred, deferred), ct) != 0)
        {
            return;
        }

        var progress = new UserProblemProgress { UserId = userId, ContestId = contestId, Index = index, IsDeferred = deferred };
        db.UserProblemProgress.Add(progress);
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            db.Entry(progress).State = EntityState.Detached;
            await query.ExecuteUpdateAsync(set => set.SetProperty(p => p.IsDeferred, deferred), ct);
        }
    }

    public async Task<bool> TryStartSyncAsync(Guid userId, DateTimeOffset now, CancellationToken ct)
    {
        var sync = new UserProblemSync { UserId = userId };
        if (!await db.UserProblemSync.AnyAsync(s => s.UserId == userId, ct))
        {
            db.UserProblemSync.Add(sync);
            try
            {
                await db.SaveChangesAsync(ct);
            }
            catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                db.Entry(sync).State = EntityState.Detached;
            }
        }

        var threshold = now - SyncInterval;
        return await db.UserProblemSync.Where(s => s.UserId == userId && (s.LastAttemptAt == null || s.LastAttemptAt <= threshold))
            .ExecuteUpdateAsync(set => set.SetProperty(s => s.LastAttemptAt, now), ct) == 1;
    }

    public Task<UserProblemSync?> GetSyncAsync(Guid userId, CancellationToken ct) =>
        db.UserProblemSync.AsNoTracking().SingleOrDefaultAsync(s => s.UserId == userId, ct);

    public async Task<DateTimeOffset> MergeProgressAsync(Guid userId, DateTimeOffset attempt,
        IReadOnlyDictionary<(int ContestId, string Index), ProgressFacts> history, CancellationToken ct)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        var entries = history.Where(p => p.Value.IsSolved || p.Value.HasFailedAttempt)
            .OrderBy(p => p.Key.ContestId).ThenBy(p => p.Key.Index, StringComparer.Ordinal);

        foreach (var contest in entries.GroupBy(p => p.Key.ContestId))
        {
            foreach (var batch in contest.Chunk(ProgressBatchSize))
            {
                await MergeBatchAsync(userId, contest.Key, batch, ct);
            }
        }

        var success = DateTimeOffset.UtcNow;
        await db.UserProblemSync.Where(s => s.UserId == userId && s.LastAttemptAt == attempt)
            .ExecuteUpdateAsync(set => set.SetProperty(s => s.LastSuccessAt, success), ct);
        await transaction.CommitAsync(ct);
        return success;
    }

    private async Task MergeBatchAsync(Guid userId, int contestId,
        KeyValuePair<(int ContestId, string Index), ProgressFacts>[] batch, CancellationToken ct)
    {
        var indexes = batch.Select(entry => entry.Key.Index).ToArray();
        var existing = await db.UserProblemProgress.AsNoTracking()
            .Where(progress => progress.UserId == userId && progress.ContestId == contestId && indexes.Contains(progress.Index))
            .Select(progress => progress.Index)
            .ToListAsync(ct);
        var existingIndexes = existing.ToHashSet(StringComparer.Ordinal);

        var existingEntries = batch.Where(entry => existingIndexes.Contains(entry.Key.Index));
        foreach (var group in existingEntries.GroupBy(entry => entry.Value))
        {
            var groupIndexes = group.Select(entry => entry.Key.Index).ToArray();
            var facts = group.Key;
            await db.UserProblemProgress
                .Where(progress => progress.UserId == userId && progress.ContestId == contestId && groupIndexes.Contains(progress.Index))
                .ExecuteUpdateAsync(update => update
                    .SetProperty(progress => progress.IsSolved, progress => progress.IsSolved || facts.IsSolved)
                    .SetProperty(progress => progress.HasFailedAttempt, progress => progress.HasFailedAttempt || facts.HasFailedAttempt), ct);
        }

        var newItems = batch.Where(entry => !existingIndexes.Contains(entry.Key.Index))
            .Select(entry => new UserProblemProgress
            {
                UserId = userId,
                ContestId = entry.Key.ContestId,
                Index = entry.Key.Index,
                IsSolved = entry.Value.IsSolved,
                HasFailedAttempt = entry.Value.HasFailedAttempt
            })
            .ToArray();

        await InsertNewProgressAsync(newItems, ct);
    }

    private async Task InsertNewProgressAsync(UserProblemProgress[] newItems, CancellationToken ct)
    {
        if (newItems.Length == 0)
        {
            return;
        }

        db.UserProblemProgress.AddRange(newItems);
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            foreach (var item in newItems)
            {
                db.Entry(item).State = EntityState.Detached;
            }

            foreach (var item in newItems)
            {
                await MergeSingleAsync(item, ct);
            }
        }
    }

    private async Task MergeSingleAsync(UserProblemProgress item, CancellationToken ct)
    {
        var query = db.UserProblemProgress.Where(p => p.UserId == item.UserId && p.ContestId == item.ContestId && p.Index == item.Index);
        if (await query.ExecuteUpdateAsync(set => set.SetProperty(p => p.IsSolved, p => p.IsSolved || item.IsSolved)
            .SetProperty(p => p.HasFailedAttempt, p => p.HasFailedAttempt || item.HasFailedAttempt), ct) != 0)
        {
            return;
        }

        db.UserProblemProgress.Add(item);
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            db.Entry(item).State = EntityState.Detached;
            await query.ExecuteUpdateAsync(set => set.SetProperty(p => p.IsSolved, p => p.IsSolved || item.IsSolved)
                .SetProperty(p => p.HasFailedAttempt, p => p.HasFailedAttempt || item.HasFailedAttempt), ct);
        }
    }
}
