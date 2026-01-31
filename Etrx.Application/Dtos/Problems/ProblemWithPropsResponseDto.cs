namespace Etrx.Application.Dtos.Problems;

public record ProblemWithPropsResponseDto(
    IEnumerable<ProblemResponseDto> Problems,
    IEnumerable<string> Properties,
    int PageCount);
