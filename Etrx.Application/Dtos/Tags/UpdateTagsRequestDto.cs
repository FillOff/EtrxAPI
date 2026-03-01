namespace Etrx.Application.Dtos.Tags;

public record UpdateTagsRequestDto
{
    public IList<UpdateTagRequestDto> Tags { get; init; } = [];
}