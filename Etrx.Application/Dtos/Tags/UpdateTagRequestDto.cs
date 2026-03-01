namespace Etrx.Application.Dtos.Tags;

public record UpdateTagRequestDto
{
    public Guid Id { get; init; }
    public int Priority { get; init; }
}