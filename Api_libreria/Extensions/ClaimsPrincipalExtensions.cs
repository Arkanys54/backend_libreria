using System.Security.Claims;

namespace Api_libreria.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Obtiene el id del usuario autenticado a partir del claim del token JWT.
    /// La identidad se toma siempre del token, nunca del cuerpo de la solicitud.
    /// </summary>
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? user.FindFirstValue("sub");

        return Guid.TryParse(value, out var id)
            ? id
            : throw new InvalidOperationException("El token no contiene un identificador de usuario válido.");
    }
}
