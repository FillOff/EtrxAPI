using Etrx.Application.Dtos.Users;
using FluentValidation;

namespace Etrx.Application.Validators;

public class CreateUserRequestDtoValidator : AbstractValidator<CreateUserRequestDto>
{
    public CreateUserRequestDtoValidator()
    {
        RuleFor(x => x.Handle)
            .NotEmpty().WithMessage("Handle can not be empty")
            .MaximumLength(100).WithMessage("Handle must not exceed 100 characters");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("FirstName can not be empty");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("LastName can not be empty");

        RuleFor(x => x.Organization)
            .NotEmpty().WithMessage("Organization can not be empty");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City can not be empty");

        RuleFor(x => x.Grade)
            .GreaterThanOrEqualTo(0).WithMessage("Grade must be greater than or equal to 0");
    }
}
