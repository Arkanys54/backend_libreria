using System.ComponentModel.DataAnnotations;
using BookReviews.Domain.Entities;

namespace BookReviews.Application.DTOs.Reviews;

public class CreateReviewRequest
{
    [Range(Review.MinRating, Review.MaxRating)]
    public int Rating { get; set; }

    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string Comment { get; set; } = string.Empty;
}
