namespace Etrx.Application.Dtos.Auth;

public record SessionResponseDto(bool Authenticated, string? Handle, bool IsAdmin);
