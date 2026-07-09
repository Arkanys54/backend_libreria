namespace BookReviews.Application.DTOs.Auth;

/// <summary>Respuesta de registro/login. Nunca incluye credenciales ni el hash.</summary>
public class AuthResponse
{
    public Guid UserId { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiresAtUtc { get; init; }
}
