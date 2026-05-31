# Backend Guidelines — ASP.NET Core / Clean Architecture

Guía de referencia para iniciar y mantener el backend de una aplicación nueva siguiendo los patrones establecidos en este proyecto.

---

## Stack tecnológico

| Capa       | Tecnología                     |
| ---------- | ------------------------------ |
| Runtime    | .NET 10 / ASP.NET Core 10      |
| ORM        | EF Core 10                     |
| Mediator   | MediatR                        |
| Validación | FluentValidation               |
| Mapeo      | AutoMapper                     |
| Auth       | ASP.NET Core Identity + JWT    |
| Logging    | Serilog                        |
| Testing    | xUnit + FluentAssertions + Moq |

---

## Estructura de la solución

La solución sigue Clean Architecture con dependencias unidireccionales:

```
Domain → Application → Persistence → Api
                    → Identity
                    → Infrastructure
```

```
Solution/
├── Solution.Domain/
│   ├── Common/
│   │   └── AuditableEntity.cs      # Id, CreatedAt, UpdatedAt
│   └── Entities/
│       └── {Entity}.cs
│
├── Solution.Application/
│   ├── Features/
│   │   └── {Entity}/
│   │       ├── Commands/
│   │       │   └── Create{Entity}/
│   │       │       ├── Create{Entity}Command.cs
│   │       │       ├── Create{Entity}CommandHandler.cs
│   │       │       ├── Create{Entity}CommandValidator.cs
│   │       │       └── Create{Entity}Response.cs
│   │       └── Queries/
│   │           └── Get{Entity}List/
│   │               ├── Get{Entity}ListQuery.cs
│   │               ├── Get{Entity}ListQueryHandler.cs
│   │               └── {Entity}ListVm.cs
│   ├── Contracts/
│   │   └── Persistence/
│   │       └── I{Entity}Repository.cs
│   ├── Exceptions/
│   │   ├── NotFoundException.cs
│   │   ├── ValidationException.cs
│   │   └── BadRequestException.cs
│   ├── Profiles/
│   │   └── MappingProfile.cs
│   └── ApplicationServiceRegistration.cs
│
├── Solution.Persistence/
│   ├── Repositories/
│   │   ├── BaseRepository.cs
│   │   └── {Entity}Repository.cs
│   ├── Configurations/
│   │   └── {Entity}Configuration.cs
│   ├── Migrations/
│   ├── {Solution}DbContext.cs
│   └── PersistenceServiceRegistration.cs
│
├── Solution.Identity/
│   ├── Models/
│   │   └── ApplicationUser.cs
│   ├── Services/
│   │   └── AuthenticationService.cs
│   ├── Seed/
│   │   └── CreateFirstUser.cs
│   └── IdentityServiceExtensions.cs
│
├── Solution.Infrastructure/
│   ├── Mail/
│   │   └── EmailService.cs
│   └── InfrastructureServiceRegistration.cs
│
└── Solution.Api/
    ├── Controllers/
    │   └── {Entity}Controller.cs
    ├── Middleware/
    │   ├── ExceptionHandlerMiddleware.cs
    │   └── MiddlewareExtensions.cs
    ├── Program.cs
    └── appsettings.json
```

> **Regla estricta:** Domain y Application **no** pueden referenciar Persistence, Identity, Infrastructure ni Api. Las violaciones de esta regla se detectan en tiempo de compilación si los `.csproj` están bien configurados.

---

## Domain

### Entidades

```csharp
namespace Solution.Domain.Entities;

public class Building : AuditableEntity
{
    public Guid BuildingId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }

    // Navegación — separada de escalares con línea en blanco
    public ICollection<Floor> Floors { get; set; } = [];
}
```

- Las entidades no contienen lógica de negocio compleja; la lógica de dominio rica va en `Value Objects` o en métodos de dominio.
- Usar `AuditableEntity` como base para todas las entidades que requieran auditoría.
- Separar propiedades escalares de propiedades de navegación con una línea en blanco.
- Inicializar colecciones con `[]` (collection expressions).

### AuditableEntity

```csharp
namespace Solution.Domain.Common;

public abstract class AuditableEntity
{
    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? LastModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }
}
```

---

## Application — CQRS con MediatR

### Commands y Queries como `record`

```csharp
// Command
public record CreateBuildingCommand(string Name, string? Address)
    : IRequest<CreateBuildingResponse>;

// Query
public record GetBuildingDetailQuery(Guid BuildingId)
    : IRequest<BuildingDetailVm>;
```

### Handler con primary constructor injection

```csharp
public class CreateBuildingCommandHandler(
    IBuildingRepository repository,
    IMapper mapper)
    : IRequestHandler<CreateBuildingCommand, CreateBuildingResponse>
{
    public async Task<CreateBuildingResponse> Handle(
        CreateBuildingCommand request,
        CancellationToken cancellationToken)
    {
        var building = mapper.Map<Building>(request);
        await repository.AddAsync(building, cancellationToken);

        return new CreateBuildingResponse
        {
            BuildingId = building.BuildingId,
            Success = true,
        };
    }
}
```

### Validator

```csharp
public class CreateBuildingCommandValidator : AbstractValidator<CreateBuildingCommand>
{
    private readonly IBuildingRepository _repository;

    public CreateBuildingCommandValidator(IBuildingRepository repository)
    {
        _repository = repository;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(150).WithMessage("El nombre no puede superar los 150 caracteres.");

        RuleFor(x => x.Name)
            .MustAsync(BeUniqueName).WithMessage("Ya existe un edificio con ese nombre.");
    }

    private async Task<bool> BeUniqueName(string name, CancellationToken ct) =>
        await _repository.IsNameUniqueAsync(name, null, ct);
}
```

### Response / VM

```csharp
// Response (command) — incluye Success y mensajes de validación
public class CreateBuildingResponse : BaseResponse
{
    public Guid BuildingId { get; set; }
}

// VM (query) — clase plana, sin comportamiento
public class BuildingDetailVm
{
    public Guid BuildingId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public int FloorCount { get; set; }
}
```

### Registro de servicios

Cada capa expone un método de extensión estático para el `IServiceCollection`:

```csharp
// Application/ApplicationServiceRegistration.cs
public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        // Pipeline behavior para validación automática
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
```

---

## Persistence

### BaseRepository

```csharp
public class BaseRepository<T>(RoomRentalsDbContext context) : IRepository<T>
    where T : class
{
    protected readonly RoomRentalsDbContext Context = context;

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await Context.Set<T>().FindAsync([id], ct);

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default) =>
        await Context.Set<T>().ToListAsync(ct);

    public async Task AddAsync(T entity, CancellationToken ct = default)
    {
        await Context.Set<T>().AddAsync(entity, ct);
        await Context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        Context.Entry(entity).State = EntityState.Modified;
        await Context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        Context.Set<T>().Remove(entity);
        await Context.SaveChangesAsync(ct);
    }
}
```

### Configuraciones de EF Core

```csharp
// Persistence/Configurations/BuildingConfiguration.cs
public class BuildingConfiguration : IEntityTypeConfiguration<Building>
{
    public void Configure(EntityTypeBuilder<Building> builder)
    {
        builder.HasKey(b => b.BuildingId);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(b => b.Name)
            .IsUnique();
    }
}
```

### Migraciones

```powershell
cd api
dotnet ef migrations add {MigrationName} `
  --project Solution.Persistence `
  --startup-project Solution.Api `
  --context RoomRentalsDbContext

dotnet ef database update `
  --project Solution.Persistence `
  --startup-project Solution.Api `
  --context RoomRentalsDbContext `
  --no-build   # Requerido cuando VS tiene el API corriendo y los DLLs están bloqueados
```

---

## Api — Controllers

Los controllers son delegadores puros: reciben la request, envían al mediador, devuelven el resultado.

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BuildingsController(IMediator mediator) : ControllerBase
{
    /// <summary>Returns all buildings.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<BuildingListVm>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<BuildingListVm>>> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetBuildingsListQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>Creates a new building.</summary>
    [HttpPost]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(typeof(CreateBuildingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateBuildingResponse>> Create(
        CreateBuildingDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateBuildingCommand(dto.Name, dto.Address);
        var result = await mediator.Send(command, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetById), new { id = result.BuildingId }, result);
    }
}
```

### Middleware de excepciones

Centraliza el manejo de errores y evita try/catch en cada controller:

```csharp
// Middleware/ExceptionHandlerMiddleware.cs
public class ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException ex)
        {
            logger.LogWarning(ex, "Resource not found.");
            context.Response.StatusCode = 404;
            await context.Response.WriteAsJsonAsync(new { message = ex.Message });
        }
        catch (ValidationException ex)
        {
            logger.LogWarning(ex, "Validation failed.");
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new { errors = ex.Errors });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception.");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new { message = "Error interno del servidor." });
        }
    }
}
```

---

## Autenticación — JWT + Identity

### Flujo

1. `POST /api/auth/login` → devuelve `{ token, expiresAt, userId, email, roles[] }`
2. El cliente almacena el token y lo envía en `Authorization: Bearer {token}`.
3. Los endpoints protegidos usan `[Authorize]`; los restringidos por rol usan `[Authorize(Roles = "Manager")]`.

### Configuración mínima en Program.cs

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            ValidateLifetime = true,
        };
    });
```

---

## Logging — Serilog

```csharp
// Program.cs
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day));
```

Usar `ILogger<T>` (inyectado) en lugar de `Log` estático:

```csharp
public class CreateBuildingCommandHandler(
    IBuildingRepository repository,
    IMapper mapper,
    ILogger<CreateBuildingCommandHandler> logger)
    : IRequestHandler<CreateBuildingCommand, CreateBuildingResponse>
{
    public async Task<CreateBuildingResponse> Handle(...)
    {
        logger.LogInformation("Creating building {Name}", request.Name);
        // ...
    }
}
```

---

## Testing

### Estructura de proyectos de test

| Proyecto            | Scope                                                     |
| ------------------- | --------------------------------------------------------- |
| `Domain.Tests`      | Lógica de entidades y value objects — sin dependencias    |
| `Application.Tests` | Handlers, validators, pipeline behaviors — Moq para repos |
| `Persistence.Tests` | Repositorios — EF Core InMemory                           |
| `Api.Tests`         | Controllers — IMediator mockeado                          |
| `Identity.Tests`    | Auth services — ASP.NET Core Identity mockeado            |

### Convenciones de nombres

```
Clase de test:  {SubjectUnderTest}Tests
Método de test: {MethodOrAction}_{Scenario}_{ExpectedResult}

Ejemplo:
  CreateBuildingCommandHandlerTests
  Handle_NameNotUnique_ReturnsFalseWithValidationError
```

### Estructura AAA

```csharp
[Fact]
public async Task Handle_NameNotUnique_ReturnsFalseWithValidationError()
{
    // Arrange
    var repo = new Mock<IBuildingRepository>();
    repo.Setup(r => r.IsNameUniqueAsync(It.IsAny<string>(), null, default))
        .ReturnsAsync(false);
    var handler = new CreateBuildingCommandHandler(repo.Object, CreateMapper());

    // Act
    var result = await handler.Handle(
        new CreateBuildingCommand("Dup", null), CancellationToken.None);

    // Assert
    result.Success.Should().BeFalse();
    result.ValidationErrors.Should().ContainSingle();
}

// Helper privado — sin base class compartida
private static IMapper CreateMapper() =>
    new MapperConfiguration(
        cfg => cfg.AddProfile<MappingProfile>(),
        NullLoggerFactory.Instance).CreateMapper();
```

### Persistence — DB única por test

```csharp
private static RoomRentalsDbContext CreateContext() =>
    new(new DbContextOptionsBuilder<RoomRentalsDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options);
```

---

## Convenciones C# — resumen rápido

- `var` solo cuando el tipo es obvio por la derecha (expresiones `new`, casts explícitos).
- Allman braces — la llave abre y cierra en su propia línea.
- File-scoped namespaces: `namespace Foo.Bar;`
- `async`/`await` para todo I/O. Nunca `.Result` ni `.Wait()`.
- Colecciones modernas: `string[] items = ["a", "b"]`, spread `[.. query.Select(...)]`.
- No usar `.ToList()` al final de LINQ cuando el tipo destino ya es conocido; sí usar `.ToListAsync()` para ejecutar queries de EF Core.
- Catch solo excepciones específicas; nunca `catch (Exception)` sin filtro.

---

## Checklist para una entidad nueva

```
□ Domain/Entities/{Entity}.cs                         — clase con AuditableEntity
□ Application/Contracts/Persistence/I{Entity}Repository.cs
□ Application/Features/{Entity}/Commands/Create.../   — command + handler + validator + response
□ Application/Features/{Entity}/Commands/Update.../
□ Application/Features/{Entity}/Commands/Delete.../
□ Application/Features/{Entity}/Queries/GetList/      — query + handler + VM
□ Application/Features/{Entity}/Queries/GetDetail/
□ Application/Profiles/MappingProfile.cs              — agregar maps
□ Persistence/Repositories/{Entity}Repository.cs
□ Persistence/Configurations/{Entity}Configuration.cs
□ Persistence/{Solution}DbContext.cs                  — agregar DbSet<{Entity}>
□ Api/Controllers/{Entity}sController.cs
□ Migrations (Add + Update)
□ Tests: Application.Tests + Persistence.Tests + Api.Tests
```
