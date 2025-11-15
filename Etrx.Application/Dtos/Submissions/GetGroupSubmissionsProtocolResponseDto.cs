namespace Etrx.Application.Dtos.Submissions;

public class GetGroupSubmissionsProtocolResponseDto
{
    public string Handle { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public int ContestId { get; set; }
    public int SolvedCount { get; set; }
}
