using BookReviews.Domain.Entities;
using BookReviews.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookReviews.Infrastructure.Persistence;

/// <summary>
/// Contexto de datos de la aplicación. Deriva de IdentityDbContext para incluir
/// las tablas de usuarios/roles de Identity junto al modelo de dominio.
/// </summary>
public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Category>(entity =>
        {
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            // Índice para optimizar la búsqueda por nombre de categoría.
            entity.HasIndex(c => c.Name);
        });

        builder.Entity<Book>(entity =>
        {
            entity.Property(b => b.Title).IsRequired().HasMaxLength(300);
            entity.Property(b => b.Author).IsRequired().HasMaxLength(200);
            entity.Property(b => b.Summary).HasMaxLength(4000);

            // Índices para optimizar la búsqueda por título y autor.
            entity.HasIndex(b => b.Title);
            entity.HasIndex(b => b.Author);

            entity.HasOne(b => b.Category)
                  .WithMany(c => c.Books)
                  .HasForeignKey(b => b.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Review>(entity =>
        {
            entity.Property(r => r.Comment).IsRequired().HasMaxLength(2000);

            entity.HasOne(r => r.Book)
                  .WithMany(b => b.Reviews)
                  .HasForeignKey(r => r.BookId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Relación con el usuario autor (sin navegación desde Review para
            // mantener el dominio libre de dependencias del framework de Identity).
            entity.HasOne<ApplicationUser>()
                  .WithMany(u => u.Reviews)
                  .HasForeignKey(r => r.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Una única reseña por usuario y por libro.
            entity.HasIndex(r => new { r.BookId, r.UserId }).IsUnique();

            // Restricción a nivel de base de datos: calificación entre 1 y 5.
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Review_Rating",
                $"\"Rating\" >= {Review.MinRating} AND \"Rating\" <= {Review.MaxRating}"));
        });
    }
}
