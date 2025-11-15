using Etrx.Domain.Models;

namespace Etrx.Application.Repositories;

public interface IProblemTranslationsRepository : IGenericRepository<ProblemTranslation>
{
    Task<List<ProblemTranslation>> GetByProblemIdsAndLanguageAsync(List<Guid> problemIds, string languageCode);
}