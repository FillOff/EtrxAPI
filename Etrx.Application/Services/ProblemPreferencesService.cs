using System.Text.Json;
using Etrx.Application.Constants;
using Etrx.Application.Dtos.Problems;
using Etrx.Application.Interfaces;
using Etrx.Application.Repositories;

namespace Etrx.Application.Services;

public sealed class ProblemPreferencesService(IProblemPreferencesRepository repository) : IProblemPreferencesService
{
    public async Task<ProblemPreferencesResponseDto> GetAsync(Guid userId, CancellationToken ct)
    {
        var json = await repository.GetAsync(userId, ct);
        if (json is null)
        {
            return new ProblemPreferencesResponseDto(new Dictionary<string, string>());
        }

        var filters = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
            ?? throw new InvalidDataException("Saved problem preferences must be an object.");
        return new ProblemPreferencesResponseDto(filters);
    }

    public async Task<PreferenceSaveStatus> SaveAsync(Guid userId, string? currentHandle,
        SaveProblemPreferencesRequestDto request, CancellationToken ct)
    {
        if (!string.Equals(currentHandle, request.ExpectedHandle, StringComparison.OrdinalIgnoreCase))
        {
            return PreferenceSaveStatus.AccountChanged;
        }

        const int maxFilterValueLength = 4096;
        var filters = request.Filters;
        if (filters is null || filters.Count > ProblemPreferenceKeys.Allowed.Count ||
            filters.Any(filter => !ProblemPreferenceKeys.Allowed.Contains(filter.Key) ||
                filter.Value is null || filter.Value.Length > maxFilterValueLength))
        {
            return PreferenceSaveStatus.InvalidFilters;
        }

        var nonEmptyFilters = filters.Where(filter => !string.IsNullOrEmpty(filter.Value))
            .ToDictionary(filter => filter.Key, filter => filter.Value);
        await repository.SaveAsync(userId, JsonSerializer.Serialize(nonEmptyFilters), ct);
        return PreferenceSaveStatus.Success;
    }
}
