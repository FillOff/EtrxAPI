using Etrx.Application.Constants;

namespace Etrx.Application.Exceptions.BadRequest;

public class InvalidLanguageException : BadRequestException
{
    public InvalidLanguageException() 
        : base($"Invalid language. Lang must be one of: {string.Join(", ", Languages.GetAll())}")
    { }
}