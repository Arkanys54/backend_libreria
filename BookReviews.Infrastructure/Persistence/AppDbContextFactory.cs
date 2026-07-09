using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BookReviews.Infrastructure.Persistence;

/// <summary>
/// Fábrica en tiempo de diseño usada por las herramientas de EF Core
/// (dotnet ef migrations / database update). Toma la cadena de conexión de la
/// variable de entorno BOOKREVIEWS_CONNECTION; si no existe usa un valor local
/// por defecto. Crear una migración no requiere conexión real; aplicarla sí.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("BOOKREVIEWS_CONNECTION")
            ?? "Host=localhost;Port=5432;Database=bookreviews;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new AppDbContext(options);
    }
}
