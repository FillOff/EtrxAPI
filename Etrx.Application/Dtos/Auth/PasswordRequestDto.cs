using Etrx.Application.Constants;
using System.ComponentModel.DataAnnotations;

namespace Etrx.Application.Dtos.Auth;

public record PasswordRequestDto(
    [Required, StringLength(RequestLimits.MaxHandleLength)] string Handle,
    [Required, StringLength(RequestLimits.MaxPasswordLength, MinimumLength = RequestLimits.MinPasswordLength)] string Password,
    bool Confirmed = false,
    [StringLength(RequestLimits.MaxActivationCodeLength)] string? ActivationCode = null);
