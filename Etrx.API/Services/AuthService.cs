using System.Security.Cryptography;
using System.Text;
using Etrx.Application.Constants;
using Etrx.Application.Dtos.Auth;
using Etrx.Application.Exceptions;
using Etrx.Application.Exceptions.BadRequest;
using Etrx.Application.Exceptions.NotFound;
using Etrx.Application.Interfaces;
using Etrx.Application.Options;
using Etrx.Application.Repositories;
using Etrx.Application.Repositories.UnitOfWork;
using Etrx.Persistence.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Etrx.API.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUsersRepository _usersRepository;
    private readonly ITransactionExecutor _transactionExecutor;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtTokenService _tokenService;
    private readonly IAuthSession _session;
    private readonly AuthOptions _authOptions;
    private readonly IValidator<HandleRequestDto> _handleValidator;
    private readonly IValidator<PasswordRequestDto> _passwordValidator;

    public AuthService(IUsersRepository usersRepository, ITransactionExecutor transactionExecutor,
        UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        JwtTokenService tokenService, IAuthSession session, IOptions<AuthOptions> authOptions,
        IValidator<HandleRequestDto> handleValidator, IValidator<PasswordRequestDto> passwordValidator)
    {
        _usersRepository = usersRepository;
        _transactionExecutor = transactionExecutor;
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _session = session;
        _authOptions = authOptions.Value;
        _handleValidator = handleValidator;
        _passwordValidator = passwordValidator;
    }

    public async Task<AuthLookupResponseDto> LookupAsync(HandleRequestDto request, CancellationToken cancellationToken = default)
    {
        _handleValidator.ValidateAndThrow(request);
        var profile = await _usersRepository.FindByHandleAsync(request.Handle, cancellationToken)
            ?? throw new NotFoundException("User not found.");
        var account = await _userManager.FindByIdAsync(profile.Id.ToString());
        var hasPassword = account is not null && await _userManager.HasPasswordAsync(account);
        return new AuthLookupResponseDto(profile.Handle, hasPassword, hasPassword ? null : profile.FirstName,
            hasPassword ? null : profile.LastName, hasPassword ? null : profile.Grade,
            _authOptions.IsAdmin(profile.Handle) && account?.AdminActivated != true);
    }

    public async Task<SessionResponseDto> SetupAsync(PasswordRequestDto request, CancellationToken cancellationToken = default)
    {
        _passwordValidator.ValidateAndThrow(request);
        var profile = await _usersRepository.FindByHandleAsync(request.Handle, cancellationToken)
            ?? throw new NotFoundException("User not found.");
        if (!request.Confirmed)
        {
            throw new BadRequestException("Confirm your details first.");
        }
        if (await _userManager.FindByIdAsync(profile.Id.ToString()) is not null)
        {
            throw new ConflictException("Password already exists. Sign in.");
        }
        var isAdmin = _authOptions.IsAdmin(profile.Handle);
        if (isAdmin && !ValidActivationCode(request.ActivationCode))
        {
            throw new ForbiddenException("The administrator activation code is invalid or not configured.");
        }
        var account = new ApplicationUser
        {
            Id = profile.Id, UserName = profile.Handle, AdminActivated = isAdmin, LockoutEnabled = true
        };
        var issuedTokens = await _transactionExecutor.ExecuteAsync(async () =>
        {
            var result = await _userManager.CreateAsync(account, request.Password);
            if (!result.Succeeded)
            {
                if (result.Errors.Any(error => error.Code == nameof(IdentityErrorDescriber.DuplicateUserName)))
                {
                    throw new ConflictException("Password already exists. Sign in.");
                }
                throw new BadRequestException(string.Join(" ", result.Errors.Select(error => error.Description)));
            }
            if (isAdmin)
            {
                IdentityRoleInitializer.EnsureSuccess(await _userManager.AddToRoleAsync(account, AuthorizationPolicies.Admin));
            }
            return await _tokenService.CreateAsync(account, cancellationToken);
        }, cancellationToken);
        return _tokenService.CompleteSignIn(issuedTokens);
    }

    public async Task<SessionResponseDto> LoginAsync(PasswordRequestDto request, CancellationToken cancellationToken = default)
    {
        _passwordValidator.ValidateAndThrow(request);
        var profile = await _usersRepository.FindByHandleAsync(request.Handle, cancellationToken);
        var account = profile is null ? null : await _userManager.FindByIdAsync(profile.Id.ToString());
        if (account is null || profile is null)
        {
            throw new UnauthorizedException("Invalid handle or password.");
        }
        var result = await _signInManager.CheckPasswordSignInAsync(account, request.Password, lockoutOnFailure: true);
        if (result.IsLockedOut)
        {
            throw new TooManyRequestsException("Too many attempts. Try again in 15 minutes.");
        }
        if (!result.Succeeded)
        {
            throw new UnauthorizedException("Invalid handle or password.");
        }
        if (await _userManager.GetTwoFactorEnabledAsync(account))
        {
            throw new ForbiddenException("Two-factor authentication is required.");
        }
        var issuedTokens = await _transactionExecutor.ExecuteAsync(async () =>
        {
            if (_authOptions.IsAdmin(profile.Handle) && !account.AdminActivated)
            {
                if (!ValidActivationCode(request.ActivationCode))
                {
                    throw new ForbiddenException("Invalid administrator activation code.");
                }
                account.AdminActivated = true;
                IdentityRoleInitializer.EnsureSuccess(await _userManager.UpdateAsync(account));
                IdentityRoleInitializer.EnsureSuccess(await _userManager.AddToRoleAsync(account, AuthorizationPolicies.Admin));
            }
            if (!string.Equals(account.UserName, profile.Handle, StringComparison.Ordinal))
            {
                IdentityRoleInitializer.EnsureSuccess(await _userManager.SetUserNameAsync(account, profile.Handle));
            }
            return await _tokenService.CreateAsync(account, cancellationToken);
        }, cancellationToken);
        return _tokenService.CompleteSignIn(issuedTokens);
    }

    public SessionResponseDto GetSession() => _session.Current;

    public Task<SessionResponseDto> RefreshAsync(CancellationToken cancellationToken = default) =>
        _tokenService.RefreshAsync(cancellationToken);

    public Task LogoutAsync(CancellationToken cancellationToken = default) =>
        _tokenService.RevokeAsync(_session.UserId, cancellationToken);

    private bool ValidActivationCode(string? code)
    {
        var expectedCode = _authOptions.AdminActivationCode;
        return !string.IsNullOrWhiteSpace(expectedCode) && expectedCode.Length >= RequestLimits.MinActivationCodeLength &&
            CryptographicOperations.FixedTimeEquals(SHA256.HashData(Encoding.UTF8.GetBytes(expectedCode)),
                SHA256.HashData(Encoding.UTF8.GetBytes(code ?? string.Empty)));
    }
}
