namespace Etrx.API.Security;

public sealed class AuthenticationLimits
{
    public int MinimumSigningKeyBytes { get; init; }
    public int RandomTokenBytes { get; init; }
    public int MaximumRefreshTokenLength { get; init; }
    public int JwtClockSkewSeconds { get; init; } = -1;
    public int AuthRequestsPerWindow { get; init; }
    public int RateLimitWindowMinutes { get; init; }
    public int MinimumAccessTokenMinutes { get; init; }
    public int MaximumAccessTokenMinutes { get; init; }
    public int MinimumRefreshTokenHours { get; init; }
    public int MaximumRefreshTokenHours { get; init; }
    public int PasswordWorkFactor { get; init; }

    public void Validate()
    {
        if (MinimumSigningKeyBytes < 32 || RandomTokenBytes < 32)
        {
            throw new InvalidOperationException("HS256 signing keys and random tokens must use at least 32 bytes.");
        }

        // Base64 expands each group of three random bytes to four characters.
        if (MaximumRefreshTokenLength < 4 * ((RandomTokenBytes + 2L) / 3))
        {
            throw new InvalidOperationException("MaximumRefreshTokenLength must fit the generated Base64 refresh token.");
        }

        if (JwtClockSkewSeconds < 0 || AuthRequestsPerWindow < 1 || RateLimitWindowMinutes < 1)
        {
            throw new InvalidOperationException("Configure a nonnegative JWT clock skew and positive rate limits.");
        }

        if (MinimumAccessTokenMinutes < 1 || MaximumAccessTokenMinutes < MinimumAccessTokenMinutes ||
            MinimumRefreshTokenHours < 1 || MaximumRefreshTokenHours < MinimumRefreshTokenHours)
        {
            throw new InvalidOperationException("Configure positive, ordered access and refresh token lifetime limits.");
        }

        if (PasswordWorkFactor is < 4 or > 31)
        {
            throw new InvalidOperationException("The BCrypt work factor must be between 4 and 31.");
        }
    }
}
