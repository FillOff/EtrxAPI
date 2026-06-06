using System.Net;

namespace Etrx.Application.Exceptions.Api;

public class CodeforcesApiException : ApiException
{
    public CodeforcesApiException(string comment, HttpStatusCode? statusCode, string url)
        : base(comment, statusCode, url)
    { }
}
