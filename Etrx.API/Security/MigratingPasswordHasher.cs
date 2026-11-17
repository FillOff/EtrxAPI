using Etrx.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Etrx.API.Security;

// UserManager owns password operations. This Identity extension preserves the required BCrypt format.
public sealed class MigratingPasswordHasher : IPasswordHasher<ApplicationUser>
{
    private const string HashPrefix = "{bcrypt-sha384}";
    private readonly PasswordHasher<ApplicationUser> _legacyHasher;

    public MigratingPasswordHasher(IOptions<PasswordHasherOptions> options)
    {
        _legacyHasher = new PasswordHasher<ApplicationUser>(options);
    }

    public string HashPassword(ApplicationUser user, string password)
    {
        return HashPrefix + BCrypt.Net.BCrypt.EnhancedHashPassword(password, AuthenticationLimits.PasswordWorkFactor);
    }

    public PasswordVerificationResult VerifyHashedPassword(ApplicationUser user, string hashedPassword, string providedPassword)
    {
        try
        {
            if (hashedPassword.StartsWith(HashPrefix, StringComparison.Ordinal))
            {
                var bcryptHash = hashedPassword[HashPrefix.Length..];
                if (!BCrypt.Net.BCrypt.EnhancedVerify(providedPassword, bcryptHash))
                {
                    return PasswordVerificationResult.Failed;
                }
                return BCrypt.Net.BCrypt.PasswordNeedsRehash(bcryptHash, AuthenticationLimits.PasswordWorkFactor)
                    ? PasswordVerificationResult.SuccessRehashNeeded : PasswordVerificationResult.Success;
            }
            var legacyResult = _legacyHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
            return legacyResult == PasswordVerificationResult.Failed
                ? PasswordVerificationResult.Failed : PasswordVerificationResult.SuccessRehashNeeded;
        }
        catch (Exception exception) when (exception is FormatException or ArgumentException or BCrypt.Net.SaltParseException)
        {
            return PasswordVerificationResult.Failed;
        }
    }
}
