namespace Etrx.Application.Dtos.Auth;

public record AuthLookupResponseDto(string Handle, bool HasPassword, string? FirstName, string? LastName, int? Grade, bool RequiresActivationCode);
