namespace BookReviews.Application.DTOs.Books;

/// <summary>Parámetros de búsqueda, filtro y paginación del listado de libros.</summary>
public class BookQueryParameters
{
    private const int MaxPageSize = 50;
    private int _pageSize = 10;
    private int _page = 1;

    /// <summary>Texto libre; coincide con título o autor.</summary>
    public string? Search { get; set; }

    /// <summary>Filtro opcional por categoría.</summary>
    public int? CategoryId { get; set; }

    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1 ? 1 : (value > MaxPageSize ? MaxPageSize : value);
    }
}
