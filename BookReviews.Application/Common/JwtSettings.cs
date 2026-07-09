namespace BookReviews.Application.Common;

/// <summary>
/// Parámetros de configuración del token JWT. La clave (Key) se provee fuera del
/// código fuente (user-secrets en desarrollo, variables de entorno en producción).
/// </summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 120;
}
