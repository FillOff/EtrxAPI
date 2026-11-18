using Etrx.Application.Constants;
using System.ComponentModel.DataAnnotations;

namespace Etrx.Application.Dtos.Problems;

public record SaveProblemPreferencesRequestDto([Required] Dictionary<string, string>? Filters,
    [Required, StringLength(RequestLimits.MaxHandleLength)] string ExpectedHandle);
