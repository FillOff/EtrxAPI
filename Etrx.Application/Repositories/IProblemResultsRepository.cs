using Etrx.Domain.Models;

namespace Etrx.Application.Repositories;

public interface IProblemResultsRepository : IGenericRepository<ProblemResult>  
{
    Task<List<ProblemResult>> GetByRanklistRowIdsAsync(List<Guid> ranklistRowIds);
}