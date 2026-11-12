using Etrx.Application.Repositories;
using Etrx.Domain.Models;
using Etrx.Persistence.Databases;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Etrx.Persistence.Repositories;

public sealed class ProblemPreferencesRepository(EtrxDbContext db) : IProblemPreferencesRepository
{
    public Task<string?> GetAsync(Guid userId, CancellationToken ct) => db.UserProblemPreferences.AsNoTracking()
        .Where(p => p.UserId == userId).Select(p => p.FiltersJson).SingleOrDefaultAsync(ct);

    public async Task SaveAsync(Guid userId, string json, CancellationToken ct)
    {
        var query = db.UserProblemPreferences.Where(p => p.UserId == userId);
        if (await query.ExecuteUpdateAsync(set => set.SetProperty(p => p.FiltersJson, json), ct) != 0)
        {
            return;
        }

        var preference = new UserProblemPreferences { UserId = userId, FiltersJson = json };
        db.UserProblemPreferences.Add(preference);
        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            db.Entry(preference).State = EntityState.Detached;
            await query.ExecuteUpdateAsync(set => set.SetProperty(p => p.FiltersJson, json), ct);
        }
    }
}
