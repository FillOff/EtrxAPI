namespace Etrx.Application.Exceptions.BadRequest;

public class UserAlreadyExistsException : BadRequestException
{
    public UserAlreadyExistsException(string handle)
        : base($"User with handle {handle} already exists")
    { }
}
