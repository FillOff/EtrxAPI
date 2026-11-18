using Etrx.Application.Constants;
using System.ComponentModel.DataAnnotations;

namespace Etrx.Application.Dtos.Problems;

public sealed record SetDeferredRequestDto(bool Deferred, [Required, StringLength(RequestLimits.MaxHandleLength)] string ExpectedHandle);
