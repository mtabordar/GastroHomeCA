"# GastroHomeCA - Quick Reference Guide

This document provides essential context for new sessions to understand the project architecture and coding patterns without needing to re-analyze the codebase.

---

## 🏗️ Architecture Overview

**Pattern:** Clean Architecture / Onion Architecture with ASP.NET Minimal APIs

**Layers (from center outward):**
1. **Domain** - Pure business logic, entities, value objects (no dependencies)
2. **Application** - Use cases, commands, queries, DTOs, mappings (depends only on Domain)
3. **Infrastructure** - Data access, external services (depends on Application + Domain)
4. **Web** - API endpoints, presentation layer (depends on all inner layers)
5. **Shared** - Common utilities, extension methods
6. **ServiceDefaults** - Telemetry, logging, health checks, DI extensions

---

## 📁 Actual Folder Structure

### Domain (`src/Domain/`)
```
Domain/
├── Entities/           # Domain entities (e.g., Product.cs)
├── Common/             # BaseEntity, shared domain concepts
└── Events/             # Domain event definitions
```

### Application (`src/Application/`)
```
Application/
├── Common/
│   ├── Interfaces/     # IApplicationDbContext, ISender, ICurrentUser
│   ├── DTOs/           # CreateProductDto, ProductDto, etc.
│   └── Behaviours/     # MediatR pipeline behaviors
├── [Feature]/          # Commands, Queries, Handlers, Mappings per feature
│   ├── Commands/       # Command definitions & handlers
│   ├── Queries/        # Query definitions & handlers
│   └── Mappings/       # Entity ↔ DTO mappings (static methods)
└── DependencyInjection.cs
```

### Infrastructure (`src/Infrastructure/`)
```
Infrastructure/
├── Data/               # ApplicationDbContext, migrations
├── Services/           # External service implementations
└── Extensions/         # Infrastructure extensions
```

### Web (`src/Web/`)
```
Web/
├── Endpoints/          # IEndpointGroup implementations (auto-discovered)
│   ├── TodoItems.cs
│   ├── TodoLists.cs
│   ├── Users.cs
│   └── [YourFeature].cs
├── Infrastructure/     # WebApplicationExtensions.MapEndpoints()
├── Services/           # CurrentUser implementation
└── DependencyInjection.cs
```

### AppHost (`src/AppHost/`)
```
AppHost/
└── DependencyInjection.cs  # Composition root for all services
```

---

## 🔑 Key Design Patterns

### 1. **Endpoint Groups (Minimal APIs)**
Endpoints are auto-discovered via `IEndpointGroup` interface.

**Pattern:**
```csharp
public class Products : IEndpointGroup
{
    public static string? RoutePrefix => null; // Defaults to /api/Products
    
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(CreateProduct);
        groupBuilder.MapGet("/{id:int}", GetProduct);
    }
    
    public static async Task<Created<int>> CreateProduct(ISender sender, CreateProductCommand command)
    {
        var id = await sender.Send(command);
        return TypedResults.Created($"/api/Products/{id}", id);
    }
}
```

**Auto-registered by:** `app.MapEndpoints(typeof(Program).Assembly)` in `Program.cs`

### 2. **Command Pattern (MediatR)**
Commands are handled by `IRequestHandler<TCommand, TResult>` or `IRequestHandler<TCommand>`.

**Handler Pattern:**
```csharp
public class CreateProductCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<CreateProductCommand, int>
{
    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product();
        product.Create(request.Name, request.Category, request.Barcode, request.CurrentPrice);
        
        await dbContext.Products.AddAsync(product);
        await dbContext.SaveChangesAsync();
        return (int)product.Id;
    }
}
```

### 3. **Domain Validation**
Business rules live in domain entities, not handlers.

**Example:**
```csharp
public void Create(string name, string category, string? barcode = null, decimal price = 0m)
{
    if (string.IsNullOrWhiteSpace(name)) 
        throw new ArgumentException("Product name cannot be empty.", nameof(name));
    if (string.IsNullOrWhiteSpace(category)) 
        throw new ArgumentException("Category cannot be empty.", nameof(category));
    
    Name = name;
    Category = category;
    // ...
}
```

### 4. **FluentValidation**
Validators use FluentValidation and are auto-registered.

```csharp
public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters.");
        
        RuleFor(x => x.CurrentPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to zero.");
    }
}
```

### 5. **Entity Mappings**
Mappings are static methods in `Application/[Feature]/Mappings/`

```csharp
public static class ProductMappings
{
    public static CreateProductDto ToCreateProductDto(Product product) => new()
    {
        Name = product.Name,
        Category = product.Category,
        Barcode = product.Barcode,
        CurrentPrice = product.CurrentPrice
    };
    
    public static ProductDto ToProductDto(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Category = product.Category,
        Barcode = product.Barcode,
        CurrentPrice = product.CurrentPrice,
        CreatedDate = product.CreatedDate,
        LastUpdatedDate = product.LastUpdatedDate
    };
}
```

---

## 📦 Key Dependencies by Layer

| Layer | NuGet Packages |
|-------|---------------|
| **Domain** | None (pure) |
| **Application** | MediatR, FluentValidation, AutoMapper (for maps) |
| **Infrastructure** | Microsoft.EntityFrameworkCore, EntityFrameworkCore.Sqlite |
| **Web** | Scalar.AspNetCore (OpenAPI docs) |

---

## 🎯 File Naming Conventions

| Type | Pattern | Example |
|------|---------|---------|
| **Entity** | `[Noun].cs` | `Product.cs`, `TodoItem.cs` |
| **Command** | `Create[Name]Command.cs` | `CreateProductCommand.cs` |
| **Query** | `[Name]Query.cs` | `ListAllProductsQuery.cs` |
| **Handler** | `[Action][Name]Handler.cs` | `CreateProductCommandHandler.cs` |
| **Validator** | `[Name]Validator.cs` | `CreateProductCommandValidator.cs` |
| **DTO** | `[Name]Dto.cs` | `ProductDto.cs`, `CreateProductDto.cs` |
| **Mapping** | `[Name]Mappings.cs` | `ProductMappings.cs` |

---

## 📝 Coding Standards

### ✅ DO:
- Use `init-only` properties for DTOs
- Keep domain entities pure (no external dependencies)
- Use static methods for entity ↔ DTO mappings
- Place validators in Application layer with FluentValidation
- Follow dependency direction (outer layers depend on inner)
- Use primary constructors for all new classes (e.g., `class MyClass(int id)` instead of property-based initialization)

### ❌ DON'T:
- Expose domain entities directly as DTOs
- Mix infrastructure concerns with domain logic
- Use AutoMapper (use static method mappings)
- Put business rules in handlers (move to domain entities)
- Use `Guid` for IDs (project uses `int`)

---

## 🔗 Dependency Injection Patterns

### Application Layer
Uses `AddApplicationServices()` in `Application/DependencyInjection.cs`:
- Auto-registers MediatR from assembly
- Auto-registers validators from assembly
- Registers AutoMapper profiles

### Infrastructure Layer
Uses `AddInfrastructureServices()` in `Infrastructure/DependencyInjection.cs`:
- Registers DbContext
- Registers external services

### Composition Root
In `AppHost/DependencyInjection.cs`:
- Composes all layer services

---

## 🚀 API Endpoint Registration

All endpoints are auto-discovered:

```csharp
// In Program.cs
app.MapEndpoints(typeof(Program).Assembly);
```

This discovers all `IEndpointGroup` implementations and registers them as route groups.

---

## 🧪 Testing Structure

| Test Type | Location | Purpose |
|-----------|----------|---------|
| Domain Unit Tests | `tests/Domain.UnitTests/` | Entity behavior, validation |
| Application Unit Tests | `tests/Application.UnitTests/` | Handler logic, validation |
| Integration Tests | `tests/Infrastructure.IntegrationTests/` | EF Core, DB operations |
| Acceptance Tests | `tests/Web.AcceptanceTests/` | E2E API flows |

---

## 📦 Key Interfaces (Don't Change)

```csharp
// Application
IApplicationDbContext          // Database access
ISender                        // MediatR sender
ICurrentUser                   // Current user context

// Domain
BaseEntity                     // All entities inherit this
BaseEvent                      // Domain events
```

---

## ⚠️ Important Notes

1. **ID Type:** All entities use `int Id` (NOT Guid)
2. **Date Format:** Use `DateTime` (not `DateTimeOffset`)
3. **Barcode Validation:** Pattern `^[0-9\\-\\s]*$`
4. **Route Defaults:** `/api/{EndpointGroupName}` unless overridden
5. **Validation Order:** Domain → FluentValidation → Handler

---

## 🎯 Quick Start for New Features

1. **Create Domain Entity** in `Domain/Entities/[Feature]/`
2. **Create Command** in `Application/[Feature]/Commands/Create/`
3. **Create Handler** in `Application/[Feature]/Handlers/Create/`
4. **Create Validator** in `Application/[Feature]/Commands/Create/`
5. **Create Mappings** in `Application/[Feature]/Mappings/`
6. **Create DTOs** in `Application/Common/DTOs/`
7. **Create Endpoint** in `Web/Endpoints/[Feature].cs`
8. **Implement Configuration** in `Infrastructure/[Feature]/`

---

*Last Updated: Current Session*
"