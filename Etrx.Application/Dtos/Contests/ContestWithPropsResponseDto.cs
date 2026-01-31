namespace Etrx.Application.Dtos.Contests;

public record class ContestWithPropsResponseDto(
    IEnumerable<ContestResponseDto> Contests,
    IEnumerable<string> Properties,
    int PageCount);
