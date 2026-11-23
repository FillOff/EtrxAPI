using Etrx.Application.Constants;
using Etrx.Application.Options;
using Etrx.Application.Exceptions;
using Etrx.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Etrx.API.Services;

public static class IdentityRoleInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var roles = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        if (!await roles.RoleExistsAsync(AuthorizationPolicies.Admin))
        {
            EnsureSuccess(await roles.CreateAsync(new IdentityRole<Guid>(AuthorizationPolicies.Admin)));
        }

        var users = services.GetRequiredService<UserManager<ApplicationUser>>();
        var options = services.GetRequiredService<IOptions<AuthOptions>>().Value;
        foreach (var user in await users.GetUsersInRoleAsync(AuthorizationPolicies.Admin))
        {
            if (!user.AdminActivated || !options.IsAdmin(user.UserName ?? string.Empty))
            {
                EnsureSuccess(await users.RemoveFromRoleAsync(user, AuthorizationPolicies.Admin));
                EnsureSuccess(await users.UpdateSecurityStampAsync(user));
            }
        }
        if (!string.IsNullOrWhiteSpace(options.AdminHandle))
        {
            var admin = await users.FindByNameAsync(options.AdminHandle.Trim());
            if (admin?.AdminActivated == true && !await users.IsInRoleAsync(admin, AuthorizationPolicies.Admin))
            {
                EnsureSuccess(await users.AddToRoleAsync(admin, AuthorizationPolicies.Admin));
            }
        }
    }

    public static void EnsureSuccess(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            if (result.Errors.Any(error => error.Code == nameof(IdentityErrorDescriber.ConcurrencyFailure)))
            {
                throw new ConflictException("The account was changed by another request. Try again.");
            }
            throw new InvalidOperationException(string.Join(" ", result.Errors.Select(error => error.Description)));
        }
    }
}
