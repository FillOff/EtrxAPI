namespace Etrx.Domain.Models;

public sealed class RefreshToken
{
    public const int HashLength = 64;
    public string TokenHash { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string SecurityStamp { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public Guid Version { get; set; } = Guid.NewGuid();
}
