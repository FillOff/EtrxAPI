using Etrx.Application.Dtos.RanklistRows;
using Etrx.Domain.Models;
using Etrx.Application.Queries;

namespace Etrx.Application.Repositories;

public interface IRanklistRowsRepository : IGenericRepository<RanklistRow>
{
    new Task<List<RanklistRow>> GetAllAsync();
    Task<List<RanklistRow>> GetByContestIdAsync(int contestId);
    Task<List<GetRanklistRowsResponseDto>> GetByContestIdWithSortAndFilterAsync(RanklistQueryParameters parameters);
}