namespace Etrx.Domain.Models;

public class UserProblemProgress
{
    public Guid UserId { get; set; }
    public int ContestId { get; set; }
    public string Index { get; set; } = string.Empty;
    public bool IsSolved { get; set; }
    public bool HasFailedAttempt { get; set; }
    public bool IsDeferred { get; set; }
}

