using Newtonsoft.Json;

namespace Etrx.Application.Services.Api;

public abstract class ApiService
{
    protected readonly HttpClient _httpClient;

    protected ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    protected async Task<T> DeserializeAsync<T>(HttpResponseMessage response)
    {
        using var stream = await response.Content.ReadAsStreamAsync();
        using var streamReader = new StreamReader(stream);
        using var jsonReader = new JsonTextReader(streamReader);

        var serializer = new JsonSerializer();
        return serializer.Deserialize<T>(jsonReader)
            ?? throw new InvalidOperationException("Response body is empty.");
    }

    protected abstract Task<TResult> HandleRequestAsync<TResult>(string url);
}
