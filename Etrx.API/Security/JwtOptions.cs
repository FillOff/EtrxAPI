namespace Etrx.API.Security;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Issuer { get; set; } = "etrx-api";
    public string Audience { get; set; } = "etrx-client";
    public string SigningKey { get; set; } = string.Empty;
    public int AccessTokenMinutes { get; set; } = 15;
    public int RefreshTokenHours { get; set; } = 12;
}
