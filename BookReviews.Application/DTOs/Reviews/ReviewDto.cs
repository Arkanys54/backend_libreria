namespace BookReviews.Application.DTOs.Reviews;

public class ReviewDto
{
    public int Id { get; init; }
    public int Rating { get; init; }
    public string Comment { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public int BookId { get; init; }
    public Guid UserId { get; init; }
    public string UserDisplayName { get; init; } = string.Empty;
}
