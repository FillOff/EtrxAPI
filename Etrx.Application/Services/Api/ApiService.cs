using Etrx.Application.Interfaces.Api;
using Newtonsoft.Json;

namespace Etrx.Application.Services.Api;

public class ApiService(HttpClient httpClient) : IApiService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<T> GetApiDataAsync<T>(string url)
    {
        try
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
        catch (HttpRequestException e)
        {
            throw new InvalidOperationException($"HTTP error: {e.Message}");
        }
        catch (JsonException e)
        {
            throw new InvalidOperationException($"JSON deserialization error: {e.Message}");
        }
    }
}
