using Etrx.Application.Constants;
using Etrx.Application.Dtos.Common;

namespace Etrx.Application.Dtos.Problems;

public record GetSortProblemRequestDto
{
    public PaginationDto Pagination { get; set; } = new();
    public SortingDto Sorting { get; set; } = new();
    public ProblemFiltersDto Filters { get; set; } = new();
    public string? ProblemName { get; set; }
    public bool IsOnly { get; set; } = false;
    public string Lang { get; set; } = Languages.Ru;
}