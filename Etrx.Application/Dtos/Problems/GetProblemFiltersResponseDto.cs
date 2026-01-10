namespace Etrx.Application.Dtos.Problems;

public record GetProblemFiltersResponseDto
{
    public ProblemFiltersDto TotalBounds { get; set; } = new();
    public ProblemFiltersDto CurrentFilters { get; set; } = new();
}