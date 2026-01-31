using Etrx.Application.Dtos.Submissions;
using FluentValidation;

namespace Etrx.Application.Validators;

public class GetUserContestProtocolRequestDtoValidator : AbstractValidator<GetUserContestProtocolRequestDto>
{
    public GetUserContestProtocolRequestDtoValidator()
    {
        RuleFor(x => x.FYear).InclusiveBetween(1900, 2100);
        RuleFor(x => x.FMonth).InclusiveBetween(1, 12);
        RuleFor(x => x.FDay).InclusiveBetween(1, 31);

        RuleFor(x => x.TYear).InclusiveBetween(1900, 2100);
        RuleFor(x => x.TMonth).InclusiveBetween(1, 12);
        RuleFor(x => x.TDay).InclusiveBetween(1, 31);

        RuleFor(x => x)
            .Must(x => BeAValidDate(x.FYear, x.FMonth, x.FDay)).WithMessage("From date (FDay, FMonth, FYear) is not a valid date")
            .OverridePropertyName("FromDate");

        RuleFor(x => x)
            .Must(x => BeAValidDate(x.TYear, x.TMonth, x.TDay)).WithMessage("To date is not a valid date")
            .OverridePropertyName("ToDate");

        RuleFor(x => x)
            .Must(BeBeforeOrEqualTo).WithMessage("Start date must be before or equal to end date");
    }

    private bool BeAValidDate(int year, int month, int day)
    {
        return DateTime.TryParse($"{year}-{month}-{day}", out _);
    }

    private bool BeBeforeOrEqualTo(GetUserContestProtocolRequestDto dto)
    {
        if (!BeAValidDate(dto.FYear, dto.FMonth, dto.FDay) ||
            !BeAValidDate(dto.TYear, dto.TMonth, dto.TDay))
        {
            return true;
        }

        var fromDate = new DateTime(dto.FYear, dto.FMonth, dto.FDay);
        var toDate = new DateTime(dto.TYear, dto.TMonth, dto.TDay);

        return fromDate <= toDate;
    }
}