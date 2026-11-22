namespace Etrx.Domain.Models;

// Legacy migration archive only. New credentials are managed by ASP.NET Core Identity.
public class UserCredential
{
    public Guid UserId { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string SecurityStamp { get; set; } = Guid.NewGuid().ToString("N");
    public bool AdminActivated { get; set; }
    public int FailedAttempts { get; set; }
    public DateTimeOffset? LockedUntil { get; set; }
}
