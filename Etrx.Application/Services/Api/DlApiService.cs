using Etrx.Application.Exceptions.Api;
using Etrx.Application.Interfaces.Api;
using Etrx.Domain.Models.ParsingModels.Dl;

namespace Etrx.Application.Services.Api;

public class DlApiService : ApiService, IDlApiService
{
    public DlApiService(HttpClient httpClient) : base(httpClient) { }

    protected override async Task<TResult> HandleRequestAsync<TResult>(string url)
    {
        using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApiException("DL Service returned an error", response.StatusCode, url);
        }

        return await DeserializeAsync<TResult>(response);
    }

    public async Task<List<DlUser>> GetDlUsersAsync()
    {
        return await HandleRequestAsync<List<DlUser>>("https://dl.gsu.by/codeforces/api/students");
    }
}
