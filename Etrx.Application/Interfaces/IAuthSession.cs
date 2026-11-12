using Etrx.Application.Dtos.Auth;

namespace Etrx.Application.Interfaces;

public interface IAuthSession
{
    Task SignInAsync(AuthenticatedUser user);
    Task SignOutAsync();
    Guid? UserId { get; }
    SessionResponseDto Current { get; }
}
