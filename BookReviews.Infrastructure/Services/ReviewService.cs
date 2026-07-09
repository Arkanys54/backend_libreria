using BookReviews.Application.Common;
using BookReviews.Application.DTOs.Reviews;
using BookReviews.Application.Interfaces;
using BookReviews.Domain.Entities;
using BookReviews.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookReviews.Infrastructure.Services;

public class ReviewService : IReviewService
{
    private readonly AppDbContext _context;

    public ReviewService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ReviewDto>> GetReviewsForBookAsync(
        int bookId, CancellationToken cancellationToken = default)
    {
        await EnsureBookExistsAsync(bookId, cancellationToken);

        // Ordenadas de la más reciente a la más antigua.
        return await _context.Reviews
            .AsNoTracking()
            .Where(r => r.BookId == bookId)
            .OrderByDescending(r => r.CreatedAt)
            .Join(_context.Users,
                  review => review.UserId,
                  user => user.Id,
                  (review, user) => new ReviewDto
                  {
                      Id = review.Id,
                      Rating = review.Rating,
                      Comment = review.Comment,
                      CreatedAt = review.CreatedAt,
                      UpdatedAt = review.UpdatedAt,
                      BookId = review.BookId,
                      UserId = review.UserId,
                      UserDisplayName = user.DisplayName
                  })
            .ToListAsync(cancellationToken);
    }

    public async Task<ReviewDto> CreateReviewAsync(
        int bookId, Guid userId, CreateReviewRequest request, CancellationToken cancellationToken = default)
    {
        await EnsureBookExistsAsync(bookId, cancellationToken);

        var alreadyReviewed = await _context.Reviews
            .AnyAsync(r => r.BookId == bookId && r.UserId == userId, cancellationToken);
        if (alreadyReviewed)
        {
            throw new ConflictException("El usuario ya publicó una reseña para este libro.");
        }

        var review = new Review
        {
            BookId = bookId,
            UserId = userId,
            Rating = request.Rating,
            Comment = request.Comment.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync(cancellationToken);

        return await MapToDtoAsync(review, cancellationToken);
    }

    public async Task<ReviewDto> UpdateReviewAsync(
        int reviewId, Guid userId, UpdateReviewRequest request, CancellationToken cancellationToken = default)
    {
        var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId, cancellationToken)
            ?? throw new NotFoundException($"No se encontró la reseña con id {reviewId}.");

        EnsureOwnership(review, userId);

        review.Rating = request.Rating;
        review.Comment = request.Comment.Trim();
        review.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return await MapToDtoAsync(review, cancellationToken);
    }

    public async Task DeleteReviewAsync(int reviewId, Guid userId, CancellationToken cancellationToken = default)
    {
        var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId, cancellationToken)
            ?? throw new NotFoundException($"No se encontró la reseña con id {reviewId}.");

        EnsureOwnership(review, userId);

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureBookExistsAsync(int bookId, CancellationToken cancellationToken)
    {
        var exists = await _context.Books.AnyAsync(b => b.Id == bookId, cancellationToken);
        if (!exists)
        {
            throw new NotFoundException($"No se encontró el libro con id {bookId}.");
        }
    }

    private static void EnsureOwnership(Review review, Guid userId)
    {
        if (review.UserId != userId)
        {
            throw new ForbiddenAccessException("No puedes modificar reseñas de otro usuario.");
        }
    }

    private async Task<ReviewDto> MapToDtoAsync(Review review, CancellationToken cancellationToken)
    {
        var displayName = await _context.Users
            .Where(u => u.Id == review.UserId)
            .Select(u => u.DisplayName)
            .FirstAsync(cancellationToken);

        return new ReviewDto
        {
            Id = review.Id,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt,
            BookId = review.BookId,
            UserId = review.UserId,
            UserDisplayName = displayName
        };
    }
}
