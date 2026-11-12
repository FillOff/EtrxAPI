using System.ComponentModel.DataAnnotations;

namespace Etrx.Application.Dtos.Auth;

public record PasswordRequestDto(
    [Required, StringLength(100)] string Handle,
    [Required, StringLength(128, MinimumLength = 10)] string Password,
    bool Confirmed = false,
    [StringLength(512)] string? ActivationCode = null);
