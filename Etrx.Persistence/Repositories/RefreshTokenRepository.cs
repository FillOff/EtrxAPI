using Etrx.Application.Repositories;
using Etrx.Domain.Models;
using Etrx.Persistence.Databases;
using Microsoft.EntityFrameworkCore;

namespace Etrx.Persistence.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly EtrxDbContext _dbContext;

    public RefreshTokenRepository(EtrxDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<RefreshToken?> FindAsync(string tokenHash, CancellationToken cancellationToken)
    {
        return _dbContext.RefreshTokens.AsNoTracking()
            .SingleOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);
    }

    public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken)
    {
        _dbContext.RefreshTokens.Add(token);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> TryRotateAsync(RefreshToken previous, RefreshToken replacement, CancellationToken cancellationToken)
    {
        var stored = await _dbContext.RefreshTokens.SingleOrDefaultAsync(token =>
            token.TokenHash == previous.TokenHash && token.Version == previous.Version && token.RevokedAt == null,
            cancellationToken);
        if (stored is null)
        {
            _dbContext.ChangeTracker.Clear();
            return false;
        }
        stored.RevokedAt = DateTimeOffset.UtcNow;
        stored.Version = Guid.NewGuid();
        _dbContext.RefreshTokens.Add(replacement);
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            // A failed SaveChanges rolled back both writes. Do not retain the unsaved replacement.
            _dbContext.ChangeTracker.Clear();
            return false;
        }
    }
}
