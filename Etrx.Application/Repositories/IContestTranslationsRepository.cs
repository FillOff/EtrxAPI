using Etrx.Domain.Models;

namespace Etrx.Application.Repositories;

public interface IContestTranslationsRepository : IGenericRepository<ContestTranslation>
{
    Task<List<ContestTranslation>> GetByContestIdsAndLanguageAsync(List<Guid> contestIds, string languageCode);
}