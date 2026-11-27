using Etrx.Application.Repositories;
using Etrx.Domain.Models;
using Etrx.Persistence.Databases;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Etrx.Persistence.Repositories;

public sealed class ProblemPreferencesRepository : IProblemPreferencesRepository
{
    private readonly EtrxDbContext _db;

    public ProblemPreferencesRepository(EtrxDbContext db)
    {
        _db = db;
    }

    public Task<string?> GetAsync(Guid userId, CancellationToken cancellationToken, bool ladder = false) => _db.UserProblemPreferences.AsNoTracking()
        .Where(p => p.UserId == userId).Select(p => ladder ? p.LadderFiltersJson : p.FiltersJson).SingleOrDefaultAsync(cancellationToken);

    public async Task SaveAsync(Guid userId, string json, CancellationToken cancellationToken, bool ladder = false)
    {
        var query = _db.UserProblemPreferences.Where(p => p.UserId == userId);
        Task<int> UpdateAsync() => ladder
            ? query.ExecuteUpdateAsync(set => set.SetProperty(p => p.LadderFiltersJson, json), cancellationToken)
            : query.ExecuteUpdateAsync(set => set.SetProperty(p => p.FiltersJson, json), cancellationToken);
        if (await UpdateAsync() != 0)
        {
            return;
        }

        var preference = new UserProblemPreferences { UserId = userId };
        if (ladder) preference.LadderFiltersJson = json;
        else preference.FiltersJson = json;
        _db.UserProblemPreferences.Add(preference);
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            _db.Entry(preference).State = EntityState.Detached;
            await UpdateAsync();
        }
    }
}
