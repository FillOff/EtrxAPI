using System.Net;

namespace Etrx.Application.Exceptions.Api;

public class ApiException : Exception
{
    public HttpStatusCode? StatusCode { get; }
    public string Url { get; }
    public string? ResponseBody { get; }

    public ApiException(string message, HttpStatusCode? statusCode, string url, string? responseBody = null)
        : base(message)
    {
        StatusCode = statusCode;
        Url = url;
        ResponseBody = responseBody;
    }

    public override string ToString()
    {
        return $"API Error: {Message} | URL: {Url} | Status: {StatusCode}{(ResponseBody is not null ? $"\nBody: { ResponseBody}" : "")}";
    }
}
