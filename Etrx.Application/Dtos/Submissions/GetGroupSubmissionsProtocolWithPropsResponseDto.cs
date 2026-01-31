namespace Etrx.Application.Dtos.Submissions;

public record GetGroupSubmissionsProtocolWithPropsResponseDto(
    IEnumerable<GetGroupSubmissionsProtocolResponseDto> Submissions,
    IEnumerable<string> Properties);