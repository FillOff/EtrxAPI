namespace Etrx.Application.Repositories;

public interface IProblemPreferencesRepository
{
    Task<string?> GetAsync(Guid userId, CancellationToken ct);
    Task SaveAsync(Guid userId, string json, CancellationToken ct);
}
