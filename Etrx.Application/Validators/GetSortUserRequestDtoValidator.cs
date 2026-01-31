using Etrx.Application.Dtos.Users;
using Etrx.Application.Providers;
using FluentValidation;

namespace Etrx.Application.Validators;

public class GetSortUserRequestDtoValidator : AbstractValidator<GetSortUserRequestDto>
{
    public GetSortUserRequestDtoValidator()
    {
        var allowedSortFields = SortingFieldsProvider.GetSortFields<UsersResponseDto>();

        RuleFor(x => x.SortField.ToLower())
            .NotEmpty().WithMessage("SortField can not be empty")
            .Must(allowedSortFields.Contains).WithMessage($"SortField must be one of: {string.Join(", ", allowedSortFields)}");

        RuleFor(x => x.SortOrder)
            .NotNull().WithMessage("SortOrder can not be empty");
    }
}