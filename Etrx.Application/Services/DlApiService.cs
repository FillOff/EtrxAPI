using Etrx.Application.Interfaces;
using Etrx.Domain.Models.ParsingModels.Dl;
namespace Etrx.Application.Services;

public class DlApiService : IDlApiService
{
    private readonly IApiService _apiService;

    public DlApiService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<List<DlUser>> GetDlUsersAsync()
    {
        var response = await _apiService.GetApiDataAsync<List<DlUser>>("https://dl.gsu.by/codeforces/api/students");

        return response;
    }
}
