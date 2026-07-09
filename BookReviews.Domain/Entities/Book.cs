namespace BookReviews.Domain.Entities;

/// <summary>
/// Libro del catálogo. Pertenece a una única categoría y puede tener varias reseñas.
/// </summary>
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
