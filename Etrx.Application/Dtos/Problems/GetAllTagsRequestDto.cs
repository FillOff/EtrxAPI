namespace Etrx.Application.Dtos.Problems;

public record GetAllTagsRequestDto(
    int MinRating,
    int MaxRating,
    List<string>? Divisions);