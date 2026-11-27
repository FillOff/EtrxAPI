using System.Text.Json;
using Etrx.Application.Constants;
using Etrx.Application.Dtos.Problems;
using Etrx.Application.Interfaces;
using Etrx.Application.Exceptions;
using Etrx.Application.Exceptions.BadRequest;
using Etrx.Application.Repositories;

namespace Etrx.Application.Services;

public sealed class ProblemPreferencesService : IProblemPreferencesService
{
    private readonly IProblemPreferencesRepository _repository;

    public ProblemPreferencesService(IProblemPreferencesRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProblemPreferencesResponseDto> GetAsync(Guid userId, CancellationToken cancellationToken, bool ladder = false)
    {
        var json = await _repository.GetAsync(userId, cancellationToken, ladder);
        if (json is null)
        {
            return new ProblemPreferencesResponseDto(new Dictionary<string, string>());
        }

        var filters = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
            ?? throw new InvalidDataException("Saved problem preferences must be an object.");
        return new ProblemPreferencesResponseDto(filters);
    }

    public async Task SaveAsync(Guid userId, string? currentHandle,
        SaveProblemPreferencesRequestDto request, CancellationToken cancellationToken, bool ladder = false)
    {
        if (string.IsNullOrWhiteSpace(request.ExpectedHandle) || request.ExpectedHandle.Length > RequestLimits.MaxHandleLength)
        {
            throw new BadRequestException("Invalid expected handle.");
        }

        if (!string.Equals(currentHandle, request.ExpectedHandle, StringComparison.OrdinalIgnoreCase))
        {
            throw new ConflictException("The account changed. Filters were not saved.");
        }

        var allowed = ladder ? LadderPreferenceKeys.Allowed : ProblemPreferenceKeys.Allowed;
        var filters = request.Filters;
        if (filters is null || filters.Count > allowed.Count ||
            filters.Any(filter => !allowed.Contains(filter.Key) ||
                filter.Value is null || filter.Value.Length > RequestLimits.MaxFilterValueLength))
        {
            throw new BadRequestException("Invalid filters.");
        }

        var nonEmptyFilters = filters.Where(filter => !string.IsNullOrEmpty(filter.Value))
            .ToDictionary(filter => filter.Key, filter => filter.Value);
        await _repository.SaveAsync(userId, JsonSerializer.Serialize(nonEmptyFilters), cancellationToken, ladder);
    }
}
