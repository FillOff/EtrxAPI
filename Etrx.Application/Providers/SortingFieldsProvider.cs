using System.Collections.Concurrent;

namespace Etrx.Application.Providers;

public static class SortingFieldsProvider
{
    private static readonly ConcurrentDictionary<Type, IReadOnlyList<string>> _cache = new();

    public static IReadOnlyList<string> GetSortFields<T>()
    {
        return _cache.GetOrAdd(typeof(T), type =>
        {
            return type.GetProperties()
                .Select(p => p.Name.ToLower())
                .ToList()
                .AsReadOnly();
        });
    }
}