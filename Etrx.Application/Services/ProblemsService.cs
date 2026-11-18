using AutoMapper;
using Etrx.Application.Constants;
using Etrx.Application.Dtos.Problems;
using Etrx.Application.Exceptions.BadRequest;
using Etrx.Application.Exceptions;
using FluentValidation;
using Etrx.Application.Interfaces;
using Etrx.Application.Providers;
using Etrx.Application.Queries.Common;
using Etrx.Application.Repositories.UnitOfWork;
using Etrx.Application.Specifications;

namespace Etrx.Application.Services;

public class ProblemsService : IProblemsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IProblemProgressService _progressService;
    private readonly IValidator<GetSortProblemRequestDto> _validator;

    public ProblemsService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IProblemProgressService progressService,
        IValidator<GetSortProblemRequestDto> validator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _progressService = progressService;
        _validator = validator;
    }
    
    public async Task<List<ProblemResponseDto>> GetProblemsByContestIdAsync(int contestId, string lang)
    {
        if (!Languages.GetAll().Contains(lang))
        {
            throw new InvalidLanguageException();
        }

        var problems = await _unitOfWork.Problems.GetByContestIdAsync(contestId);
        var response = _mapper.Map<List<ProblemResponseDto>>(problems, opts =>
        {
            opts.Items["lang"] = lang;
        });

        return response;
    }

    public async Task<ProblemWithPropsResponseDto> GetProblemsByPageWithSortAndFilterAsync(GetSortProblemRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        ValidatePersonalFilters(dto);
        var spec = new ProblemsSpecification(dto);

        var pagedResult = await _unitOfWork.Problems.GetPagedAsync<ProblemResponseDto>(
            spec,
            new PaginationQueryParameters(dto.Pagination.Page, dto.Pagination.PageSize),
            dto.Lang, dto.CurrentUserId, dto.Statuses, dto.HideSolved, cancellationToken);

        if (dto.CurrentUserId is Guid userId)
        {
            await _progressService.EnrichAsync(userId, pagedResult.Items, cancellationToken);
        }

        return new ProblemWithPropsResponseDto
        (
            Problems: pagedResult.Items, 
            Properties: SortingFieldsProvider.GetSortFields<ProblemResponseDto>(),
            PageCount: pagedResult.TotalPagesCount
        );
    }

    public Task<GetProblemFiltersResponseDto> GetProblemFiltersAsync(GetSortProblemRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        ValidatePersonalFilters(dto);
        return _unitOfWork.Problems.GetProblemFiltersAsync(dto, cancellationToken);
    }

    private void ValidatePersonalFilters(GetSortProblemRequestDto dto)
    {
        _validator.ValidateAndThrow(dto);
        if ((dto.Statuses.Length > 0 || dto.HideSolved) && dto.CurrentUserId is null)
        {
            throw new UnauthorizedException("Sign in to filter problems by personal progress.");
        }
    }
}
