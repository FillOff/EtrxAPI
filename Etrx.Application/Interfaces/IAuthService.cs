using Etrx.Application.Dtos.Auth;

namespace Etrx.Application.Interfaces;

public interface IAuthService
{
    Task<AuthLookupResponseDto> LookupAsync(HandleRequestDto request, CancellationToken cancellationToken = default);
    Task<SessionResponseDto> SetupAsync(PasswordRequestDto request, CancellationToken cancellationToken = default);
    Task<SessionResponseDto> LoginAsync(PasswordRequestDto request, CancellationToken cancellationToken = default);
    Task<SessionResponseDto> RefreshAsync(CancellationToken cancellationToken = default);
    SessionResponseDto GetSession();
    Task LogoutAsync(CancellationToken cancellationToken = default);
}
