namespace Etrx.Application.Exceptions.BadRequest;

public class BadRequestException : RequestException
{
    public BadRequestException(string message) 
        : base(message)
    { }
}
