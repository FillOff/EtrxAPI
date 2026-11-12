using Etrx.Application.Repositories;
using Etrx.Domain.Models;
using Etrx.Persistence.Databases;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Etrx.Persistence.Repositories;

public sealed class AuthRepository(EtrxDbContext db) : IAuthRepository
{
    public Task<User?> FindUserAsync(string handle) => db.Users.AsNoTracking()
        .SingleOrDefaultAsync(user => user.Handle.ToLower() == handle.Trim().ToLower());

    public Task<User?> FindUserByIdAsync(Guid userId, CancellationToken ct) => db.Users.AsNoTracking()
        .SingleOrDefaultAsync(user => user.Id == userId, ct);

    public Task<UserCredential?> FindCredentialAsync(Guid userId, bool tracking = false)
    {
        var query = tracking ? db.UserCredentials.AsQueryable() : db.UserCredentials.AsNoTracking();
        return query.SingleOrDefaultAsync(credential => credential.UserId == userId);
    }

    public async Task<bool> AddCredentialAsync(UserCredential credential)
    {
        db.UserCredentials.Add(credential);
        try
        {
            await db.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            db.Entry(credential).State = EntityState.Detached;
            return false;
        }
    }

    public Task RecordFailedAttemptAsync(Guid userId) => db.UserCredentials.Where(c => c.UserId == userId)
        .ExecuteUpdateAsync(set => set.SetProperty(c => c.FailedAttempts, c => c.FailedAttempts + 1)
            .SetProperty(c => c.LockedUntil, c => c.FailedAttempts >= 4
                ? (DateTimeOffset?)DateTimeOffset.UtcNow.AddMinutes(15) : c.LockedUntil));

    public Task SaveAsync() => db.SaveChangesAsync();

    public Task InvalidateSessionAsync(Guid userId)
    {
        var stamp = Guid.NewGuid().ToString("N");
        return db.UserCredentials.Where(c => c.UserId == userId)
            .ExecuteUpdateAsync(set => set.SetProperty(c => c.SecurityStamp, stamp));
    }
}
