using Etrx.Application.Dtos.Auth;

namespace Etrx.Application.Interfaces;

public interface IAuthSession
{
    Guid? UserId { get; }
    SessionResponseDto Current { get; }
}
