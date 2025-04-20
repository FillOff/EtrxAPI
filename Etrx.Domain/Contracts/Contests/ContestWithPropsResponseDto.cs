namespace Etrx.Core.Contracts.Contests;

public record class ContestWithPropsResponseDto(
    List<ContestResponseDto> Contests,
    string[] Properties,
    int PageCount);
