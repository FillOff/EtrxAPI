using Etrx.Domain.Models;

namespace Etrx.Application.Repositories;

public interface IAuthRepository
{
    Task<User?> FindUserAsync(string handle);
    Task<User?> FindUserByIdAsync(Guid userId, CancellationToken ct);
    Task<UserCredential?> FindCredentialAsync(Guid userId, bool tracking = false);
    Task<bool> AddCredentialAsync(UserCredential credential);
    Task RecordFailedAttemptAsync(Guid userId);
    Task SaveAsync();
    Task InvalidateSessionAsync(Guid userId);
}
