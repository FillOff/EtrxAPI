using Etrx.Application.Constants;
using Etrx.Application.Dtos.Auth;
using FluentValidation;

namespace Etrx.Application.Validators;

public sealed class HandleRequestDtoValidator : AbstractValidator<HandleRequestDto>
{
    public HandleRequestDtoValidator()
    {
        RuleFor(request => request.Handle).NotEmpty().MaximumLength(RequestLimits.MaxHandleLength);
    }
}
