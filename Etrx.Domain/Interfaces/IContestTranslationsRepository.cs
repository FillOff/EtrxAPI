using Etrx.Domain.Models;

namespace Etrx.Domain.Interfaces;

public interface IContestTranslationsRepository : IGenericRepository<ContestTranslation>
{
    Task<List<ContestTranslation>> GetByContestIdsAndLanguageAsync(List<Guid> contestIds, string languageCode);
}