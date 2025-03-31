namespace Etrx.Core.Contracts.Contests;

public record ContestWithPropsResponseDto(
    List<ContestResponseDto> Contests,
    string[] Properties,
    int PageCount);
