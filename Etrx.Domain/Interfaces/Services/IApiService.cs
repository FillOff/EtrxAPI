namespace Etrx.Domain.Interfaces.Services;

public interface IApiService
{
    Task<T> GetApiDataAsync<T>(string url);
}