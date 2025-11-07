using Etrx.Domain.Models;

namespace Etrx.Domain.Interfaces;

public interface IProblemResultsRepository : IGenericRepository<ProblemResult>  
{
    Task<List<ProblemResult>> GetByRanklistRowIdsAsync(List<Guid> ranklistRowIds);
}