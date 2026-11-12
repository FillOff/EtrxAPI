using System.ComponentModel.DataAnnotations;

namespace Etrx.Application.Dtos.Auth;

public record HandleRequestDto([Required, StringLength(100)] string Handle);
