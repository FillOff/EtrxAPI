namespace Etrx.Core.Contracts.Problems;

public record ProblemWithPropsResponseDto(
    List<ProblemResponseDto> Problems,
    string[] Properties,
    int PageCount);
