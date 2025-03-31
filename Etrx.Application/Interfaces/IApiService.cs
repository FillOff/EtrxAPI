namespace Etrx.Application.Interfaces;

public interface IApiService
{
    Task<T> GetApiDataAsync<T>(string url);
}