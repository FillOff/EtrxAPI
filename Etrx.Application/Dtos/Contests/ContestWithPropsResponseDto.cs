namespace Etrx.Application.Dtos.Contests;

public record class ContestWithPropsResponseDto(
    List<ContestResponseDto> Contests,
    List<string> Properties,
    int PageCount);
