namespace Etrx.Application.Exceptions;

public sealed class ConflictException : RequestException
{
    public ConflictException(string message) : base(message)
    {
    }
}
