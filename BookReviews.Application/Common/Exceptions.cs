namespace BookReviews.Application.Common;

/// <summary>Recurso no encontrado. El middleware lo traduce a HTTP 404.</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

/// <summary>El usuario autenticado no tiene permiso sobre el recurso. Se traduce a HTTP 403.</summary>
public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException(string message) : base(message) { }
}

/// <summary>Conflicto con el estado actual del recurso (p. ej. duplicado). Se traduce a HTTP 409.</summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}

/// <summary>Regla de negocio incumplida en los datos de entrada. Se traduce a HTTP 400.</summary>
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message) { }
}

/// <summary>Credenciales inválidas o no autenticado. Se traduce a HTTP 401.</summary>
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) { }
}
