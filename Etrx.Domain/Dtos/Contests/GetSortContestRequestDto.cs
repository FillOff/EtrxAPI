namespace Etrx.Domain.Dtos.Contests;

public record class GetSortContestRequestDto(
    int Page = 1,
    int PageSize = 100,
    bool? Gym = null,
    string SortField = "contestid",
    bool SortOrder = true,
    string Lang = "ru");