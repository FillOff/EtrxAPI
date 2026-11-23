using Etrx.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Etrx.API.Security;

// After a successful login, UserManager migrates Identity PBKDF2 hashes and upgrades outdated BCrypt hashes.
public sealed class MigratingPasswordHasher : IPasswordHasher<ApplicationUser>
{
    private const string HashPrefix = "{bcrypt-sha384}";
    private readonly PasswordHasher<ApplicationUser> _legacyHasher;
    private readonly int _workFactor;

    public MigratingPasswordHasher(IOptions<PasswordHasherOptions> options, AuthenticationLimits limits)
    {
        _legacyHasher = new PasswordHasher<ApplicationUser>(options);
        _workFactor = limits.PasswordWorkFactor;
    }

    public string HashPassword(ApplicationUser user, string password)
    {
        return HashPrefix + BCrypt.Net.BCrypt.EnhancedHashPassword(password, _workFactor);
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
                return BCrypt.Net.BCrypt.PasswordNeedsRehash(bcryptHash, _workFactor)
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
