namespace Etrx.Domain.Dtos.Contests;

public record class ContestWithPropsResponseDto(
    List<ContestResponseDto> Contests,
    string[] Properties,
    int PageCount);
