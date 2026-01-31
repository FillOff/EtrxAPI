using Etrx.Application.Constants;
using Etrx.Application.Dtos.RanklistRows;
using Etrx.Application.Providers;
using FluentValidation;

namespace Etrx.Application.Validators;

public class GetRanklistRowsRequestDtoValidator : AbstractValidator<GetRanklistRowsRequestDto>
{
    public GetRanklistRowsRequestDtoValidator()
    {
        var allowedSortFields = SortingFieldsProvider.GetSortFields<GetRanklistRowsResponseDto>();

        RuleFor(x => x.SortField)
            .NotEmpty().WithMessage("SortField can not be empty")
            .Must(allowedSortFields.Contains).WithMessage($"SortField must be one of: {string.Join(", ", allowedSortFields)}");

        RuleFor(x => x.SortOrder)
            .NotNull().WithMessage("SortOrder can not be empty");

        RuleFor(x => x.ParticipantType)
            .NotEmpty().WithMessage("ParticipantType can not be empty")
            .Must(ParticipantTypes.GetAll().Contains).WithMessage($"ParticipantType must be one of: {string.Join(", ", ParticipantTypes.GetAll())}");

        RuleFor(x => x.Lang)
            .NotEmpty().WithMessage("Lang can not be empty")
            .Must(Languages.GetAll().Contains).WithMessage($"Lang must be one of: {string.Join(", ", Languages.GetAll())}");
    }
}
