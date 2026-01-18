using Etrx.Application.Interfaces.Api;
using Newtonsoft.Json;

namespace Etrx.Application.Services.Api;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<T> GetApiDataAsync<T>(string url)
    {
        using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        using var streamReader = new StreamReader(stream);
        using var jsonReader = new JsonTextReader(streamReader);

        var serializer = new JsonSerializer();
        return serializer.Deserialize<T>(jsonReader)
                ?? throw new InvalidOperationException("Unable to deserialize response.");
    }
}