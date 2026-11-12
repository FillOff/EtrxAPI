using System.ComponentModel.DataAnnotations;

namespace Etrx.Application.Dtos.Problems;

public sealed record SetDeferredRequestDto(bool Deferred, [Required, StringLength(100)] string ExpectedHandle);
