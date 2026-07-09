using BookReviews.Application.Common;
using BookReviews.Application.DTOs.Auth;
using BookReviews.Application.Interfaces;
using BookReviews.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BookReviews.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IJwtTokenService tokenService,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim();

        if (await _userManager.FindByEmailAsync(email) is not null)
        {
            throw new ConflictException("Ya existe un usuario registrado con ese correo.");
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            DisplayName = request.DisplayName.Trim()
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            throw new ValidationException(errors);
        }

        _logger.LogInformation("Nuevo usuario registrado: {UserId}", user.Id);
        return BuildAuthResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new UnauthorizedException("Correo o contraseña incorrectos.");
        }

        return BuildAuthResponse(user);
    }

    private AuthResponse BuildAuthResponse(ApplicationUser user)
    {
        var (token, expiresAt) = _tokenService.CreateToken(user.Id, user.Email!, user.DisplayName);

        return new AuthResponse
        {
            UserId = user.Id,
            DisplayName = user.DisplayName,
            Email = user.Email!,
            Token = token,
            ExpiresAtUtc = expiresAt
        };
    }
}
