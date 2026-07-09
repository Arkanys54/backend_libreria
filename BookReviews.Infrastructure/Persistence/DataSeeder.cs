using BookReviews.Domain.Entities;
using BookReviews.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BookReviews.Infrastructure.Persistence;

/// <summary>
/// Siembra datos iniciales (categorías, libros, usuarios y reseñas de prueba).
/// Es idempotente: solo inserta lo que aún no existe, por lo que puede ejecutarse
/// en cada arranque sin duplicar registros.
/// </summary>
public static class DataSeeder
{
    public const string DemoUserEmail = "demo@bookreviews.local";
    public const string DemoUserPassword = "Demo123!";

    /// <summary>Contraseña compartida por todos los usuarios sembrados (solo para desarrollo).</summary>
    public const string SeededUserPassword = "Demo123!";

    // Usuarios que dejan reseñas de ejemplo. El usuario demo se deja aparte y sin
    // reseñas, para poder probar la creación de una reseña propia en cualquier libro.
    private static readonly (string DisplayName, string Email)[] Reviewers =
    {
        ("Ana Torres", "ana@bookreviews.local"),
        ("Carlos Ruiz", "carlos@bookreviews.local"),
        ("Lucía Fernández", "lucia@bookreviews.local"),
        ("Miguel Santos", "miguel@bookreviews.local"),
        ("Sofía Ramírez", "sofia@bookreviews.local"),
    };

    // Distribución de calificaciones (ponderada hacia valoraciones positivas).
    private static readonly int[] RatingPool = { 5, 5, 4, 4, 4, 3, 3, 5, 4, 2, 3, 5, 4, 1 };

    private static readonly Dictionary<int, string[]> CommentsByRating = new()
    {
        [5] = new[]
        {
            "Una obra maestra, la recomiendo totalmente.",
            "Me encantó de principio a fin.",
            "Imprescindible; lo releería sin dudarlo.",
        },
        [4] = new[]
        {
            "Muy buen libro, lo disfruté bastante.",
            "Sólido y bien escrito, con algún altibajo.",
            "Gran lectura, aunque el ritmo baja a veces.",
        },
        [3] = new[]
        {
            "Está bien, cumple pero no me marcó.",
            "Entretenido, aunque esperaba un poco más.",
            "Correcto: ni me encantó ni me decepcionó.",
        },
        [2] = new[]
        {
            "Me costó terminarlo, no era del todo para mí.",
            "Tiene ideas interesantes, pero se hace lento.",
        },
        [1] = new[]
        {
            "No logró engancharme en ningún momento.",
            "Esperaba mucho más; me decepcionó.",
        },
    };

    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        var context = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(DataSeeder));

        await SeedCatalogAsync(context, logger, cancellationToken);

        // Usuario demo (sin reseñas) + usuarios reseñadores.
        await EnsureUserAsync(userManager, DemoUserEmail, "Usuario Demo", logger);
        var reviewers = new List<ApplicationUser>();
        foreach (var (displayName, email) in Reviewers)
        {
            var user = await EnsureUserAsync(userManager, email, displayName, logger);
            if (user is not null)
            {
                reviewers.Add(user);
            }
        }

        await SeedReviewsAsync(context, reviewers, logger, cancellationToken);
    }

    private static async Task SeedCatalogAsync(AppDbContext context, ILogger logger, CancellationToken cancellationToken)
    {
        if (await context.Books.AnyAsync(cancellationToken))
        {
            return;
        }

        // Categorías (reutiliza las que ya existan por nombre).
        var categoryNames = new[] { "Novela", "Ciencia ficción", "Historia", "Ensayo", "Fantasía", "Tecnología" };

        var existing = await context.Categories
            .Where(c => categoryNames.Contains(c.Name))
            .ToDictionaryAsync(c => c.Name, cancellationToken);

        foreach (var name in categoryNames)
        {
            if (!existing.ContainsKey(name))
            {
                var category = new Category { Name = name };
                context.Categories.Add(category);
                existing[name] = category;
            }
        }

        await context.SaveChangesAsync(cancellationToken);

        var books = new[]
        {
            new Book { Title = "Cien años de soledad", Author = "Gabriel García Márquez", Category = existing["Novela"],
                Summary = "La saga de la familia Buendía en el pueblo ficticio de Macondo." },
            new Book { Title = "Rayuela", Author = "Julio Cortázar", Category = existing["Novela"],
                Summary = "Una novela experimental que puede leerse en múltiples órdenes." },
            new Book { Title = "Dune", Author = "Frank Herbert", Category = existing["Ciencia ficción"],
                Summary = "Política, religión y ecología en el planeta desértico Arrakis." },
            new Book { Title = "Fundación", Author = "Isaac Asimov", Category = existing["Ciencia ficción"],
                Summary = "La psicohistoria intenta preservar el conocimiento ante la caída de un imperio galáctico." },
            new Book { Title = "Sapiens", Author = "Yuval Noah Harari", Category = existing["Historia"],
                Summary = "Una breve historia de la humanidad desde la Edad de Piedra." },
            new Book { Title = "SPQR", Author = "Mary Beard", Category = existing["Historia"],
                Summary = "Una historia de la antigua Roma y su expansión." },
            new Book { Title = "El mito de Sísifo", Author = "Albert Camus", Category = existing["Ensayo"],
                Summary = "Un ensayo sobre el absurdo y el sentido de la existencia." },
            new Book { Title = "Vigilar y castigar", Author = "Michel Foucault", Category = existing["Ensayo"],
                Summary = "El nacimiento de la prisión y las estructuras de poder." },
            new Book { Title = "El nombre del viento", Author = "Patrick Rothfuss", Category = existing["Fantasía"],
                Summary = "Kvothe narra su vida, de niño prodigio a leyenda viva." },
            new Book { Title = "El Hobbit", Author = "J. R. R. Tolkien", Category = existing["Fantasía"],
                Summary = "El viaje de Bilbo Bolsón junto a un grupo de enanos." },
            new Book { Title = "Clean Code", Author = "Robert C. Martin", Category = existing["Tecnología"],
                Summary = "Principios y prácticas para escribir código legible y mantenible." },
            new Book { Title = "The Pragmatic Programmer", Author = "Andrew Hunt, David Thomas", Category = existing["Tecnología"],
                Summary = "Consejos prácticos para el desarrollo de software profesional." }
        };

        context.Books.AddRange(books);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Catálogo inicial sembrado: {Categories} categorías, {Books} libros.",
            existing.Count, books.Length);
    }

    private static async Task SeedReviewsAsync(
        AppDbContext context, IReadOnlyList<ApplicationUser> reviewers, ILogger logger, CancellationToken cancellationToken)
    {
        if (await context.Reviews.AnyAsync(cancellationToken))
        {
            return;
        }

        var books = await context.Books.OrderBy(b => b.Id).ToListAsync(cancellationToken);
        if (books.Count == 0 || reviewers.Count == 0)
        {
            return;
        }

        // Semilla fija para que los datos generados sean reproducibles entre arranques.
        var random = new Random(20260708);
        var created = 0;
        var hoursAgo = 0;

        foreach (var book in books)
        {
            // Entre 2 y 5 reseñadores distintos por libro (una reseña por usuario/libro).
            var count = random.Next(2, Math.Min(reviewers.Count, 5) + 1);
            var selected = reviewers.OrderBy(_ => random.Next()).Take(count);

            foreach (var reviewer in selected)
            {
                var rating = RatingPool[random.Next(RatingPool.Length)];
                var pool = CommentsByRating[rating];
                var comment = pool[random.Next(pool.Length)];

                // Fechas escalonadas hacia el pasado para que el orden por fecha sea visible.
                hoursAgo += random.Next(5, 40);

                context.Reviews.Add(new Review
                {
                    BookId = book.Id,
                    UserId = reviewer.Id,
                    Rating = rating,
                    Comment = comment,
                    CreatedAt = DateTime.UtcNow.AddHours(-hoursAgo),
                });
                created++;
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Reseñas de prueba sembradas: {Count} de {Reviewers} usuarios.",
            created, reviewers.Count);
    }

    private static async Task<ApplicationUser?> EnsureUserAsync(
        UserManager<ApplicationUser> userManager, string email, string displayName, ILogger logger)
    {
        var existing = await userManager.FindByEmailAsync(email);
        if (existing is not null)
        {
            return existing;
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            DisplayName = displayName,
        };

        var result = await userManager.CreateAsync(user, SeededUserPassword);
        if (result.Succeeded)
        {
            logger.LogInformation("Usuario sembrado: {Email}", email);
            return user;
        }

        logger.LogWarning("No se pudo crear el usuario {Email}: {Errors}",
            email, string.Join("; ", result.Errors.Select(e => e.Description)));
        return null;
    }
}
