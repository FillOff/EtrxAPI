namespace Etrx.Application.Repositories;

public interface IProblemPreferencesRepository
{
    Task<string?> GetAsync(Guid userId, CancellationToken cancellationToken);
    Task SaveAsync(Guid userId, string json, CancellationToken cancellationToken);
}
