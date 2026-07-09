using BookReviews.Application.DTOs.Categories;
using BookReviews.Application.Interfaces;
using BookReviews.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookReviews.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;

    public CategoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto { Id = c.Id, Name = c.Name })
            .ToListAsync(cancellationToken);
    }
}
