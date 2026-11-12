using Etrx.Domain.Models;

namespace Etrx.Application.Dtos.Auth;

public sealed record AuthenticatedUser(User User, UserCredential Credential, bool IsAdmin);
