namespace Etrx.Domain.Contracts.Submissions;

public record class GetSubmissionsWithPropsProtocolResponseDto(
    List<GetSubmissionsProtocolResponseDto> Submissions,
    string[] Properties,
    int PageCount);