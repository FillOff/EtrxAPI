namespace Etrx.Domain.Dtos.Submissions;

public class GetGroupSubmissionsProtocolResponseDto
{
    public string UserName { get; set; } = string.Empty;
    public int ContestId { get; set; }
    public int SolvedCount { get; set; }
}
