namespace Etrx.Domain.Dtos.Problems;

public record ProblemWithPropsResponseDto(
    List<ProblemResponseDto> Problems,
    string[] Properties,
    int PageCount);
