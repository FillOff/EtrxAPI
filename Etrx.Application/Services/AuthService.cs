using Etrx.Application.Dtos.Auth;
using Etrx.Application.Interfaces;
using Etrx.Application.Options;
using Etrx.Application.Repositories;
using Etrx.Domain.Models;
using Microsoft.Extensions.Options;

namespace Etrx.Application.Services;

public sealed class AuthService(IAuthRepository repository, IAuthPasswordService passwordService,
    IHashService hashService, IOptions<AuthOptions> options, IAuthSession session) : IAuthService
{
    private const string InvalidCredentials = "Invalid handle or password.";
    private const string PasswordExists = "Password already exists. Sign in.";

    public async Task<AuthResult<AuthLookupResponseDto>> LookupAsync(HandleRequestDto request)
    {
        var user = await repository.FindUserAsync(request.Handle);
        if (user is null)
        {
            return new(AuthStatus.NotFound, Message: "User not found.");
        }

        var credential = await repository.FindCredentialAsync(user.Id);
        var hasPassword = credential is not null;
        var requiresActivationCode = options.Value.IsAdmin(user.Handle) && credential?.AdminActivated != true;
        var response = new AuthLookupResponseDto(
            Handle: user.Handle,
            HasPassword: hasPassword,
            FirstName: hasPassword ? null : user.FirstName,
            LastName: hasPassword ? null : user.LastName,
            Grade: hasPassword ? null : user.Grade,
            RequiresActivationCode: requiresActivationCode);

        return new AuthResult<AuthLookupResponseDto>(AuthStatus.Success, response);
    }

    public async Task<AuthResult<SessionResponseDto>> SetupAsync(PasswordRequestDto request)
    {
        var user = await repository.FindUserAsync(request.Handle);
        if (user is null)
        {
            return new(AuthStatus.NotFound, Message: "User not found.");
        }

        if (!request.Confirmed)
        {
            return new(AuthStatus.BadRequest, Message: "Confirm your details first.");
        }

        if (await repository.FindCredentialAsync(user.Id) is not null)
        {
            return new(AuthStatus.Conflict, Message: PasswordExists);
        }

        var isAdmin = options.Value.IsAdmin(user.Handle);
        if (isAdmin && !ValidActivationCode(request.ActivationCode))
        {
            return new(AuthStatus.Forbidden, Message: "The administrator activation code is invalid or not configured.");
        }

        var credential = new UserCredential
        {
            UserId = user.Id,
            AdminActivated = isAdmin
        };
        credential.PasswordHash = passwordService.Hash(credential, request.Password);
        if (!await repository.AddCredentialAsync(credential))
        {
            return new(AuthStatus.Conflict, Message: PasswordExists);
        }

        await session.SignInAsync(new AuthenticatedUser(user, credential, isAdmin));
        return new(AuthStatus.Success, new SessionResponseDto(true, user.Handle, isAdmin));
    }

    public async Task<AuthResult<SessionResponseDto>> LoginAsync(PasswordRequestDto request)
    {
        var user = await repository.FindUserAsync(request.Handle);
        var credential = user is null ? null : await repository.FindCredentialAsync(user.Id, true);
        if (credential is null || user is null)
        {
            return new(AuthStatus.Unauthorized, Message: InvalidCredentials);
        }

        if (credential.LockedUntil > DateTimeOffset.UtcNow)
        {
            return new(AuthStatus.TooManyRequests, Message: "Too many attempts. Try again in 15 minutes.");
        }

        var verification = passwordService.Verify(credential, request.Password);
        if (verification == PasswordVerification.Failed)
        {
            await repository.RecordFailedAttemptAsync(user.Id);
            return new(AuthStatus.Unauthorized, Message: InvalidCredentials);
        }

        var isAdmin = options.Value.IsAdmin(user.Handle);
        if (isAdmin && !credential.AdminActivated)
        {
            if (!ValidActivationCode(request.ActivationCode))
            {
                return new(AuthStatus.Forbidden, Message: "Invalid administrator activation code.");
            }

            credential.AdminActivated = true;
        }

        credential.FailedAttempts = 0;
        credential.LockedUntil = null;
        if (verification == PasswordVerification.RehashNeeded)
        {
            credential.PasswordHash = passwordService.Hash(credential, request.Password);
        }

        await repository.SaveAsync();
        isAdmin = isAdmin && credential.AdminActivated;
        await session.SignInAsync(new AuthenticatedUser(user, credential, isAdmin));
        return new(AuthStatus.Success, new SessionResponseDto(true, user.Handle, isAdmin));
    }

    public SessionResponseDto CurrentSession() => session.Current;

    public async Task LogoutAsync()
    {
        if (session.UserId is Guid userId)
        {
            await repository.InvalidateSessionAsync(userId);
        }

        await session.SignOutAsync();
    }

    private bool ValidActivationCode(string? code)
    {
        const int minimumCodeLength = 32;
        var expectedCode = options.Value.AdminActivationCode;
        return !string.IsNullOrWhiteSpace(expectedCode)
            && expectedCode.Length >= minimumCodeLength
            && hashService.Matches(expectedCode, code);
    }
}
