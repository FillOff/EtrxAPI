namespace Etrx.Application.Exceptions;

public sealed class ForbiddenException : RequestException
{
    public ForbiddenException(string message) : base(message)
    {
    }
}
