using BookReviews.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BookReviews.Infrastructure.Identity;

/// <summary>
/// Usuario de la aplicación. Extiende IdentityUser&lt;Guid&gt; para reutilizar el
/// manejo de credenciales, hashing de contraseña y confirmaciones de ASP.NET Identity.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = string.Empty;

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
