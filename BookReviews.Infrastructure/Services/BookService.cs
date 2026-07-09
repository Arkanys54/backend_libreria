using BookReviews.Application.Common;
using BookReviews.Application.DTOs.Books;
using BookReviews.Application.Interfaces;
using BookReviews.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookReviews.Infrastructure.Services;

public class BookService : IBookService
{
    private readonly AppDbContext _context;

    public BookService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<BookListItemDto>> GetBooksAsync(
        BookQueryParameters query, CancellationToken cancellationToken = default)
    {
        var books = _context.Books.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var pattern = $"%{query.Search.Trim()}%";
            books = books.Where(b =>
                EF.Functions.ILike(b.Title, pattern) ||
                EF.Functions.ILike(b.Author, pattern));
        }

        if (query.CategoryId is int categoryId)
        {
            books = books.Where(b => b.CategoryId == categoryId);
        }

        var totalItems = await books.CountAsync(cancellationToken);

        var items = await books
            .OrderBy(b => b.Title)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(b => new BookListItemDto
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                CategoryName = b.Category.Name,
                ReviewCount = b.Reviews.Count,
                AverageRating = b.Reviews.Any() ? b.Reviews.Average(r => r.Rating) : 0
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<BookListItemDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems
        };
    }

    public async Task<BookDetailDto> GetBookByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var book = await _context.Books
            .AsNoTracking()
            .Where(b => b.Id == id)
            .Select(b => new BookDetailDto
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Summary = b.Summary,
                CategoryId = b.CategoryId,
                CategoryName = b.Category.Name,
                ReviewCount = b.Reviews.Count,
                AverageRating = b.Reviews.Any() ? b.Reviews.Average(r => r.Rating) : 0
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (book is null)
        {
            throw new NotFoundException($"No se encontró el libro con id {id}.");
        }

        return book;
    }
}
