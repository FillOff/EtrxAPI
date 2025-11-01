namespace Etrx.Domain.Dtos.Problems;

public record GetAllTagsRequestDto(
    int MinRating,
    int MaxRating);