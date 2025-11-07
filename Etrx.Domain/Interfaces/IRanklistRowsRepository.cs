using Etrx.Domain.Dtos.RanklistRows;
using Etrx.Domain.Models;
using Etrx.Domain.Queries;

namespace Etrx.Domain.Interfaces;

public interface IRanklistRowsRepository : IGenericRepository<RanklistRow>
{
    new Task<List<RanklistRow>> GetAllAsync();
    Task<List<RanklistRow>> GetByContestIdAsync(int contestId);
    Task<List<GetRanklistRowsResponseDto>> GetByContestIdWithSortAndFilterAsync(RanklistQueryParameters parameters);
}