namespace Etrx.Application.Exceptions.NotFound;

public class NotFoundException : RequestException
{
    public NotFoundException(string message) 
        : base(message)
    { }
}
