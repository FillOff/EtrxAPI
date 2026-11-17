using Etrx.Domain.Models;

namespace Etrx.Application.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> FindAsync(string tokenHash, CancellationToken cancellationToken);
    Task AddAsync(RefreshToken token, CancellationToken cancellationToken);
    Task<bool> TryRotateAsync(RefreshToken previous, RefreshToken replacement, CancellationToken cancellationToken);
}
