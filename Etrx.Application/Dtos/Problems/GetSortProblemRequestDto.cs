using Etrx.Application.Constants;
using Etrx.Application.Dtos.Common;

namespace Etrx.Application.Dtos.Problems;

public record GetSortProblemRequestDto
{
    public PaginationDto Pagination { get; set; } = new PaginationDto();
    public SortingDto Sorting { get; set; } = new SortingDto() { SortField = "contestid", SortOrder = SortOrders.Asc };
    public ProblemFiltersDto Filters { get; set; } = new ProblemFiltersDto();
    public string? ProblemName { get; set; }
    public bool IsOnly { get; set; } = false;
    public string Lang { get; set; } = Languages.Ru;
    public string[] Statuses { get; set; } = [];
    public bool HideSolved { get; set; }

    // Always overwritten by ProblemsController from the authenticated claims.
    [System.Text.Json.Serialization.JsonIgnore]
    public Guid? CurrentUserId { get; set; }
}
