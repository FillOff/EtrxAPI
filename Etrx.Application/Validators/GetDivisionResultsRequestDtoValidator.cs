using Etrx.Application.Constants;
using Etrx.Application.Dtos.Submissions;
using Etrx.Application.Exceptions.BadRequest;
using Etrx.Application.Services;
using FluentValidation;

namespace Etrx.Application.Validators;

public class GetDivisionResultsRequestDtoValidator : AbstractValidator<GetDivisionResultsRequestDto>
{
    public GetDivisionResultsRequestDtoValidator()
    {
        RuleFor(x => x).Custom((dto, context) =>
        {
            try
            {
                ProtocolDateRange.ToUnix(dto.FYear, dto.FMonth, dto.FDay,
                    dto.TYear, dto.TMonth, dto.TDay);
            }
            catch (BadRequestException ex) { context.AddFailure("Dates", ex.Message); }
        });
        RuleFor(x => x.Division).Must(value => ProtocolResultsCalculator.Divisions.Contains(value))
            .WithMessage("Допустимые дивизионы: ALL, Div1, Div2, Div3, Div4.");
        RuleFor(x => x.ParticipantType).Must(value => value == ParticipantTypes.All
                || ProtocolResultsCalculator.Types.Contains(value))
            .WithMessage("Некорректный тип участия.");
        RuleFor(x => x.ContestId).GreaterThan(0).When(x => x.ContestId.HasValue);
    }
}
