namespace Etrx.Application.Interfaces.Api;

public interface IApiService
{
    Task<T> GetApiDataAsync<T>(string url);
}