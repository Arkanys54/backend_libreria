using Api_libreria.Extensions;
using BookReviews.Application.DTOs.Reviews;
using BookReviews.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api_libreria.Controllers;

[ApiController]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet("api/books/{bookId:int}/reviews")]
    [ProducesResponseType(typeof(IReadOnlyList<ReviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ReviewDto>>> GetForBook(int bookId, CancellationToken cancellationToken)
    {
        var reviews = await _reviewService.GetReviewsForBookAsync(bookId, cancellationToken);
        return Ok(reviews);
    }

    [Authorize]
    [HttpPost("api/books/{bookId:int}/reviews")]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ReviewDto>> Create(
        int bookId, CreateReviewRequest request, CancellationToken cancellationToken)
    {
        var review = await _reviewService.CreateReviewAsync(bookId, User.GetUserId(), request, cancellationToken);
        return CreatedAtAction(nameof(GetForBook), new { bookId }, review);
    }

    [Authorize]
    [HttpPut("api/reviews/{id:int}")]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReviewDto>> Update(
        int id, UpdateReviewRequest request, CancellationToken cancellationToken)
    {
        var review = await _reviewService.UpdateReviewAsync(id, User.GetUserId(), request, cancellationToken);
        return Ok(review);
    }

    [Authorize]
    [HttpDelete("api/reviews/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _reviewService.DeleteReviewAsync(id, User.GetUserId(), cancellationToken);
        return NoContent();
    }
}
