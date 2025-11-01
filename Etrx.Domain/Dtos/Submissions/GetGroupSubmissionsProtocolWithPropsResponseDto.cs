namespace Etrx.Domain.Dtos.Submissions;

public record GetGroupSubmissionsProtocolWithPropsResponseDto(
    List<GetGroupSubmissionsProtocolResponseDto> Submissions,
    List<string> Properties);