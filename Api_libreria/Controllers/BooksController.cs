using BookReviews.Application.Common;
using BookReviews.Application.DTOs.Books;
using BookReviews.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api_libreria.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<BookListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<BookListItemDto>>> GetBooks(
        [FromQuery] BookQueryParameters query, CancellationToken cancellationToken)
    {
        var result = await _bookService.GetBooksAsync(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BookDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDetailDto>> GetBook(int id, CancellationToken cancellationToken)
    {
        var book = await _bookService.GetBookByIdAsync(id, cancellationToken);
        return Ok(book);
    }
}
