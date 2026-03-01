namespace Etrx.Application.Dtos.Tags;

public record TagsResponseDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int Priority { get; init; }
}