namespace Etrx.Domain.Dtos.Common;

public class PagedResultDto<T>
{
    public List<T> Items { get; set; } = [];

    public int TotalPagesCount { get; set; }
}
