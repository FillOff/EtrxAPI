using Etrx.Domain.Models;

namespace Etrx.Application.Interfaces;

public interface IAuthPasswordService
{
    string Hash(UserCredential credential, string password);
    PasswordVerification Verify(UserCredential credential, string password);
}
