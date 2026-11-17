using Etrx.Application.Constants;
using Etrx.Application.Dtos.Auth;
using FluentValidation;

namespace Etrx.Application.Validators;

public sealed class PasswordRequestDtoValidator : AbstractValidator<PasswordRequestDto>
{
    public PasswordRequestDtoValidator()
    {
        RuleFor(request => request.Handle).NotEmpty().MaximumLength(RequestLimits.MaxHandleLength);
        RuleFor(request => request.Password).NotEmpty()
            .Length(RequestLimits.MinPasswordLength, RequestLimits.MaxPasswordLength);
        RuleFor(request => request.ActivationCode).MaximumLength(RequestLimits.MaxActivationCodeLength);
    }
}
