using Etrx.Domain.Models;

namespace Etrx.Domain.Interfaces;

public interface IProblemTranslationsRepository : IGenericRepository<ProblemTranslation>
{
    Task<List<ProblemTranslation>> GetByProblemIdsAndLanguageAsync(List<Guid> problemIds, string languageCode);
}