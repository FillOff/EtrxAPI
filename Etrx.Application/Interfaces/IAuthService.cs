using Etrx.Application.Dtos.Auth;

namespace Etrx.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResult<AuthLookupResponseDto>> LookupAsync(HandleRequestDto request);
    Task<AuthResult<SessionResponseDto>> SetupAsync(PasswordRequestDto request);
    Task<AuthResult<SessionResponseDto>> LoginAsync(PasswordRequestDto request);
    SessionResponseDto CurrentSession();
    Task LogoutAsync();
}
