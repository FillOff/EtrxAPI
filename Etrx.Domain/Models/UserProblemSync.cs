namespace Etrx.Domain.Models;

public class UserProblemSync
{
    public Guid UserId { get; set; }
    public DateTimeOffset? LastAttemptAt { get; set; }
    public DateTimeOffset? LastSuccessAt { get; set; }
}
