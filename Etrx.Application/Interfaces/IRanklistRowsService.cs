using Etrx.Application.Dtos.RanklistRows;

namespace Etrx.Application.Interfaces;

public interface IRanklistRowsService
{
    Task<GetRanklistRowsResponseWithPropsDto> GetRanklistRowsWithSortAsync(int contestId, GetRanklistRowsRequestDto dto);
}