using Etrx.Application.Constants;
using System.ComponentModel.DataAnnotations;

namespace Etrx.Application.Dtos.Auth;

public record HandleRequestDto([Required, StringLength(RequestLimits.MaxHandleLength)] string Handle);
