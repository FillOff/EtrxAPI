namespace Etrx.Application.Dtos.Problems;

public record ProblemWithPropsResponseDto(
    List<ProblemResponseDto> Problems,
    List<string> Properties,
    int PageCount);
