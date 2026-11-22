namespace Etrx.Application.Exceptions;

public sealed class UnauthorizedException : RequestException
{
    public UnauthorizedException(string message) : base(message)
    {
    }
}
