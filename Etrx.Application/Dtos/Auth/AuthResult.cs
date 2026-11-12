namespace Etrx.Application.Dtos.Auth;

public sealed record AuthResult<T>(AuthStatus Status, T? Value = default, string? Message = null);
