namespace Etrx.Application.Dtos.Common;

public record PaginationDto
{
    public int Page { get; set; }
    public int PageSize { get; set; }
}