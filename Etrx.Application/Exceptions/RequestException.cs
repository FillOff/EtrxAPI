namespace Etrx.Application.Exceptions;

public abstract class RequestException : Exception
{
    protected RequestException(string message) : base(message)
    {
    }
}
