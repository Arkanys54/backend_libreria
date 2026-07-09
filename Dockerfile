# ---------- Etapa de compilación ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar los .csproj y restaurar primero (aprovecha la caché de capas de Docker).
COPY Api_libreria/Api_libreria.csproj Api_libreria/
COPY BookReviews.Domain/BookReviews.Domain.csproj BookReviews.Domain/
COPY BookReviews.Application/BookReviews.Application.csproj BookReviews.Application/
COPY BookReviews.Infrastructure/BookReviews.Infrastructure.csproj BookReviews.Infrastructure/
RUN dotnet restore Api_libreria/Api_libreria.csproj

# Copiar el resto del código y publicar la API.
COPY . .
RUN dotnet publish Api_libreria/Api_libreria.csproj -c Release -o /app/publish /p:UseAppHost=false

# ---------- Etapa de ejecución ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# El puerto real lo inyecta el host (Render) mediante la variable de entorno PORT,
# que Program.cs usa para configurar Kestrel.
ENTRYPOINT ["dotnet", "Api_libreria.dll"]
