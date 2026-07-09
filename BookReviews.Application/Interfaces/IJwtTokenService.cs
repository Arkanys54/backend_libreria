namespace BookReviews.Application.Interfaces;

/// <summary>Genera tokens JWT para un usuario autenticado.</summary>
public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAtUtc) CreateToken(Guid userId, string email, string displayName);
}
