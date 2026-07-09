using BookReviews.Application.DTOs.Reviews;

namespace BookReviews.Application.Interfaces;

public interface IReviewService
{
    Task<IReadOnlyList<ReviewDto>> GetReviewsForBookAsync(int bookId, CancellationToken cancellationToken = default);

    Task<ReviewDto> CreateReviewAsync(int bookId, Guid userId, CreateReviewRequest request, CancellationToken cancellationToken = default);

    Task<ReviewDto> UpdateReviewAsync(int reviewId, Guid userId, UpdateReviewRequest request, CancellationToken cancellationToken = default);

    Task DeleteReviewAsync(int reviewId, Guid userId, CancellationToken cancellationToken = default);
}
