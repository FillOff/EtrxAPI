namespace Etrx.Application.Dtos.Common;

public record SortingDto
{
    public string SortField { get; set; } = string.Empty;
    public string SortOrder { get; set; } = string.Empty;
}