using Etrx.Domain.Models.ParsingModels.Codeforces;

namespace Etrx.Application.Exceptions;

public class CodeforcesApiException : Exception
{
    public CodeforcesApiException(CodeforcesResponse response) 
        : base(response.Comment)
    { }
}