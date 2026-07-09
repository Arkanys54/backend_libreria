# BookReviews — Backend

API REST de una aplicación de reseñas de libros: catálogo con búsqueda, filtro y paginación,
detalle de libros y publicación de reseñas (1–5 estrellas + comentario) con autenticación.

## Stack

ASP.NET Core 8 Web API · Entity Framework Core · PostgreSQL · ASP.NET Identity + JWT ·
Swagger/OpenAPI · xUnit.

## Estructura (arquitectura en capas)

```
Api_libreria/               Capa API: controllers, Program.cs, middleware
BookReviews.Domain/         Entidades y enumeraciones
BookReviews.Application/    DTOs, interfaces, validaciones
BookReviews.Infrastructure/ EF Core, Identity, servicios, migraciones, seeder
BookReviews.Tests/          Pruebas unitarias
```

## Puesta en marcha local

### Requisitos
- .NET SDK 8+
- PostgreSQL

### Pasos
1. Crea `Api_libreria/appsettings.Development.json` a partir de
   `Api_libreria/appsettings.Development.json.example` y completa la cadena de conexión y `Jwt:Key`.
   (Alternativa recomendada: `dotnet user-secrets`.)
2. Ejecuta la API:
   ```bash
   dotnet run --project Api_libreria
   ```
   Al arrancar aplica las migraciones y siembra datos de ejemplo. Swagger disponible en `/swagger`.

### Cuentas de ejemplo (sembradas)
- Demo (sin reseñas): `demo@bookreviews.local` / `Demo123!`
- Reseñadores: `ana@bookreviews.local`, `carlos@…`, `lucia@…`, `miguel@…`, `sofia@…` (misma contraseña)

## Endpoints principales

| Método | Ruta | Auth | Descripción |
|--------|------|------|-------------|
| POST | `/api/auth/register` | No | Registro |
| POST | `/api/auth/login` | No | Login (devuelve JWT) |
| GET | `/api/books` | No | Listado (búsqueda, filtro, paginación) |
| GET | `/api/books/{id}` | No | Detalle |
| GET | `/api/categories` | No | Categorías |
| GET | `/api/books/{id}/reviews` | No | Reseñas del libro |
| POST | `/api/books/{id}/reviews` | Sí | Crear reseña |
| PUT | `/api/reviews/{id}` | Sí | Editar reseña propia |
| DELETE | `/api/reviews/{id}` | Sí | Eliminar reseña propia |

## Despliegue

Variables de entorno en producción: `ConnectionStrings__Default`, `Jwt__Key`, `Jwt__Issuer`,
`Jwt__Audience`, `Cors__AllowedOrigins__0` (URL del frontend).
