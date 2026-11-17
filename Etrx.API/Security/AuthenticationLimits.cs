namespace Etrx.API.Security;

public static class AuthenticationLimits
{
    public const int MinimumSigningKeyBytes = 32;
    public const int RandomTokenBytes = 64;
    public const int MaximumRefreshTokenLength = 256;
    public const int JwtClockSkewSeconds = 15;
    public const int AuthRequestsPerWindow = 20;
    public const int RateLimitWindowMinutes = 1;
    public const int MinimumAccessTokenMinutes = 1;
    public const int MaximumAccessTokenMinutes = 60;
    public const int MinimumRefreshTokenHours = 1;
    public const int MaximumRefreshTokenHours = 720;
    public const int PasswordWorkFactor = 12;
}
