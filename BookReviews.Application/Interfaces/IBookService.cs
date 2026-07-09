using BookReviews.Application.Common;
using BookReviews.Application.DTOs.Books;

namespace BookReviews.Application.Interfaces;

public interface IBookService
{
    Task<PagedResult<BookListItemDto>> GetBooksAsync(BookQueryParameters query, CancellationToken cancellationToken = default);
    Task<BookDetailDto> GetBookByIdAsync(int id, CancellationToken cancellationToken = default);
}
