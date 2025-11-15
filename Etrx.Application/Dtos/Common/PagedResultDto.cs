namespace Etrx.Application.Dtos.Common;

public class PagedResultDto<T>
{
    public List<T> Items { get; init; } = [];
    public int TotalPagesCount { get; init; }
    public int TotalItemsCount { get; init; }
}