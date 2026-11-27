namespace Etrx.Application.Repositories;

public interface IProblemPreferencesRepository
{
    Task<string?> GetAsync(Guid userId, CancellationToken cancellationToken, bool ladder = false);
    Task SaveAsync(Guid userId, string json, CancellationToken cancellationToken, bool ladder = false);
}
