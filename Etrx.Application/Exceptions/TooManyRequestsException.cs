namespace Etrx.Application.Exceptions;

public sealed class TooManyRequestsException : RequestException
{
    public TooManyRequestsException(string message) : base(message)
    {
    }
}
