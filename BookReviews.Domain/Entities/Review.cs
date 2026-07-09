namespace BookReviews.Domain.Entities;

/// <summary>
/// Reseña de un libro escrita por un usuario. La calificación se restringe a 1..5
/// (validado en la aplicación y con una restricción CHECK en la base de datos).
/// </summary>
public class Review
{
    public const int MinRating = 1;
    public const int MaxRating = 5;

    public int Id { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public int BookId { get; set; }
    public Book Book { get; set; } = null!;

    /// <summary>
    /// Identificador del autor de la reseña. Corresponde al usuario de Identity
    /// (ApplicationUser). La identidad se toma siempre del token JWT, nunca del cuerpo.
    /// </summary>
    public Guid UserId { get; set; }
}
