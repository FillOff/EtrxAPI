using Etrx.Application.Constants;
using Etrx.Application.Dtos.Contests;
using Etrx.Application.Providers;
using FluentValidation;

namespace Etrx.Application.Validators;

public class GetSortContestRequestDtoValidator : AbstractValidator<GetSortContestRequestDto>
{
    private const int MAX_PAGE_SIZE = 100;

    public GetSortContestRequestDtoValidator()
    {
        var allowedSortFields = SortingFieldsProvider.GetSortFields<ContestResponseDto>();

        RuleFor(x => x.Page)
            .NotEmpty().WithMessage("Page can not be empty")
            .GreaterThan(0).WithMessage("Page number must be greater than zero");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than zero")
            .LessThanOrEqualTo(MAX_PAGE_SIZE).WithMessage($"Page size must not exceed {MAX_PAGE_SIZE}");

        RuleFor(x => x.SortField.ToLower())
            .NotEmpty().WithMessage("SortField can not be empty")
            .Must(allowedSortFields.Contains).WithMessage($"SortField must be one of: {string.Join(", ", allowedSortFields)}");

        RuleFor(x => x.SortOrder)
            .NotNull().WithMessage("SortOrder can not be empty");

        RuleFor(x => x.Lang)
            .NotEmpty().WithMessage("Lang can not be empty")
            .Must(Languages.GetAll().Contains).WithMessage($"Lang must be one of: {string.Join(", ", Languages.GetAll())}");
    }
}