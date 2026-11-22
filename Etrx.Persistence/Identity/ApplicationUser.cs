using Microsoft.AspNetCore.Identity;

namespace Etrx.Persistence.Identity;

// The key matches the imported profile. Importing Codeforces data cannot overwrite credentials.
public sealed class ApplicationUser : IdentityUser<Guid>
{
    public bool AdminActivated { get; set; }
}
