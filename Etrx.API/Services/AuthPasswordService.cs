using Etrx.Application.Interfaces;
using Etrx.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Etrx.API.Services;

public sealed class AuthPasswordService(IPasswordHasher<UserCredential> hasher) : IAuthPasswordService
{
    public string Hash(UserCredential credential, string password) => hasher.HashPassword(credential, password);

    public PasswordVerification Verify(UserCredential credential, string password) =>
        hasher.VerifyHashedPassword(credential, credential.PasswordHash, password) switch
        {
            PasswordVerificationResult.Success => PasswordVerification.Success,
            PasswordVerificationResult.SuccessRehashNeeded => PasswordVerification.RehashNeeded,
            _ => PasswordVerification.Failed
        };
}
