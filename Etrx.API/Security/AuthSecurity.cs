using System.Security.Claims;
using Etrx.Application.Constants;
using Etrx.Application.Options;
using Etrx.Domain.Models;

namespace Etrx.API.Security;

public static class AuthSecurity
{
    public const string Scheme = "EtrxSession";
    public const string SecurityStampClaim = "stamp";

    public static ClaimsPrincipal CreatePrincipal(User user, UserCredential account, AuthOptions options)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Handle),
            new(SecurityStampClaim, account.SecurityStamp)
        };

        if (account.AdminActivated && options.IsAdmin(user.Handle))
        {
            claims.Add(new Claim(ClaimTypes.Role, AuthorizationPolicies.Admin));
        }

        return new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme));
    }
}
