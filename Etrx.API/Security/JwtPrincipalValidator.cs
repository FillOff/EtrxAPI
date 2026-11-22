using System.Security.Claims;
using Etrx.Application.Repositories;
using Etrx.Persistence.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;

namespace Etrx.API.Security;

public sealed class JwtPrincipalValidator
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUsersRepository _usersRepository;

    public JwtPrincipalValidator(SignInManager<ApplicationUser> signInManager, IUsersRepository usersRepository)
    {
        _signInManager = signInManager;
        _usersRepository = usersRepository;
    }

    public async Task ValidateAsync(TokenValidatedContext context)
    {
        var user = await _signInManager.ValidateSecurityStampAsync(context.Principal);
        if (user is null || await _signInManager.UserManager.IsLockedOutAsync(user) || !await _signInManager.CanSignInAsync(user))
        {
            context.Fail("Account or security stamp is no longer valid.");
            return;
        }
        var roles = await _signInManager.UserManager.GetRolesAsync(user);
        if (!roles.ToHashSet(StringComparer.Ordinal).SetEquals(context.Principal!.FindAll(ClaimTypes.Role).Select(claim => claim.Value)))
        {
            context.Fail("Account roles have changed.");
            return;
        }
        var profile = await _usersRepository.FindByIdAsync(user.Id, context.HttpContext.RequestAborted);
        if (profile is null)
        {
            context.Fail("Profile no longer exists.");
            return;
        }
        var identity = (ClaimsIdentity)context.Principal.Identity!;
        var name = identity.FindFirst(ClaimTypes.Name);
        if (name is not null)
        {
            identity.RemoveClaim(name);
        }
        identity.AddClaim(new Claim(ClaimTypes.Name, profile.Handle));
    }
}
