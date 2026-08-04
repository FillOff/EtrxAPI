using Etrx.Application.Constants;

namespace Etrx.Application.Dtos.Contests;

public record class GetSortContestRequestDto(
    int Page = 1,
    int PageSize = 100,
    string? ContestId = null,
    bool? Gym = null,
    string? Source = null,
    string SortField = "contestid",
    bool SortOrder = true,
    string Lang = Languages.Ru);