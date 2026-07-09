using System.ComponentModel.DataAnnotations;

namespace BookReviews.Application.DTOs.Auth;

public class RegisterRequest
{
    [Required]
    [StringLength(80, MinimumLength = 2)]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}
