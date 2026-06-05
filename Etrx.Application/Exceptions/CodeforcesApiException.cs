namespace Etrx.Application.Exceptions;

public class CodeforcesApiException : Exception
{
    public CodeforcesApiException(string comment) 
        : base(comment)
    { }
}
