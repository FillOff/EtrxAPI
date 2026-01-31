namespace Etrx.Application.Exceptions.BadRequest;

public class BadRequestException : Exception
{
    public BadRequestException(string message) 
        : base(message)
    { }
}
