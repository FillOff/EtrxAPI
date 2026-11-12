namespace Etrx.Application.Dtos.Auth;

public enum AuthStatus
{
    Success,
    NotFound,
    BadRequest,
    Conflict,
    Forbidden,
    Unauthorized,
    TooManyRequests
}
