namespace BookReviews.Application.DTOs.Books;

/// <summary>Proyección liviana de un libro para el listado (sin resumen completo).</summary>
public class BookListItemDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public string CategoryName { get; init; } = string.Empty;
    public double AverageRating { get; init; }
    public int ReviewCount { get; init; }
}
