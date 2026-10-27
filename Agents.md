# GastroHomeCA - Quick Reference Guide

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
├── Entities/           # Domain entities (Product.cs)
├── Common/             # BaseEntity, BaseAuditableEntity, shared domain concepts
├── Events/             # Domain event definitions
├── ValueObjects/       # Immutable value objects (Colour.cs)
├── Enums/              # Enumeration types (PriorityLevel.cs)
├── Constants/          # Global constants (Roles.cs)
└── Exceptions/         # Domain exceptions
```

### Application (`src/Application/`)
```
Application/
├── Common/
│   ├── Interfaces/     # IApplicationDbContext, ISender, ICurrentUser, IIdentityService
│   ├── DTOs/           # CreateProductDto, ProductDto, etc.
│   ├── Behaviours/     # MediatR pipeline behaviors (AuthorizationBehaviour)
│   ├── Exceptions/     # NotFoundException, ValidationException, etc.
│   ├── Models/         # Result<T>, LookupDto
│   └── Security/       # AuthorizeAttribute
├── Product/            # Product feature
│   ├── Commands/       # CreateProductCommand, UpdateProductCommand
│   ├── Queries/        # GetProductByIdQuery, ListProductsQuery
│   └── Mappings/       # ProductMappings.cs
└── Common/             # Shared application concerns
```

### Infrastructure (`src/Infrastructure/`)
```
Infrastructure/
├── Data/               # ApplicationDbContext, migrations
│   ├── Configurations/ # Entity configurations (ProductConfiguration, etc.)
│   └── Interceptors/   # AuditableEntityInterceptor, DispatchDomainEventsInterceptor
├── Services/           # External service implementations
└── Identity/           # ASP.NET Core Identity (ApplicationUser, IdentityService)
```

### Web (`src/Web/`)
```
Web/
├── Endpoints/          # IEndpointGroup implementations (auto-discovered)
│   ├── Products.cs
│   ├── Users.cs
├── ClientApp/          # React frontend (Vite + NSwag)
│   ├── src/
│   │   ├── components/ # React components (ProductListPage, etc.)
│   │   ├── web-api-client.ts  # NSwag-generated TypeScript API client
│   │   └── api-http-client.js # HTTP client wrapper
├── Infrastructure/     # WebApplicationExtensions.MapEndpoints()
├── Services/           # CurrentUser implementation
└── Data/               # SQLite database (GastroHomeCA.db)
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
| **Web** | Scalar.AspNetCore (OpenAPI docs), NSwag |

---

## 🎯 File Naming Conventions

| Type | Pattern | Example |
|------|---------|---------|
| **Entity** | `[Noun].cs` | `Product.cs` |
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
IIdentityService              // Identity service

// Domain
BaseEntity                     // All entities inherit this
BaseAuditableEntity           // Auditable entities
BaseEvent                      // Domain events
```

---

## ⚠️ Important Notes

1. **ID Type:** All entities use `int Id` (NOT Guid)
2. **Date Format:** Use `DateTime` (not `DateTimeOffset`)
3. **Barcode Validation:** Pattern `^[0-9\s\-]*$`
4. **Route Defaults:** `/api/{EndpointGroupName}` unless overridden
5. **Validation Order:** Domain → FluentValidation → Handler
6. **Frontend:** React with NSwag-generated TypeScript client
7. **API Docs:** Scalar OpenAPI UI at `/scalar`

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
9. **Add React Component** in `Web/ClientApp/src/components/[Feature]/`

---

## 🐛 Troubleshooting Common Issues

### Build Errors

**Missing export errors:**
- Check if all types are properly exported from `web-api-client.ts`
- Ensure NSwag has generated the latest API client

**Sass deprecation warnings:**
- These are safe to ignore for now
- Consider upgrading PicoCSS in the future

### Runtime Errors

**Database initialization:**
- Only runs in Development environment
- Requires `app.InitialiseDatabaseAsync()` call

**CORS errors:**
- Current config allows any origin
- Update for production: restrict allowed origins

---

## 💻 Running Commands & Tests (Best Practices)

### Building the Project

```bash
# Always use absolute paths or proper directory changes
cd /home/marlont/Proyectos/GastroHomeCA

# Build entire solution
dotnet build

# Build specific project
dotnet build src/Web/

# Build React client
cd src/Web/ClientApp && npm run build
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/Application.UnitTests/

# Run tests with verbose output
dotnet test --verbosity normal

# Run tests with filter
dotnet test --filter "FullyQualifiedName~Product"
```

### Running the Application

```bash
# Using Aspire (recommended)
dotnet run --project src/AppHost

# Running Web project directly
dotnet run --project src/Web/

# Running with specific environment
dotnet run --project src/AppHost --environment Development
```

### Database Operations

```bash
# Add migration
cd src/Web/
dotnet ef migrations add [MigrationName]

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

### Frontend Development

```bash
# Install dependencies
cd src/Web/ClientApp
npm install

# Run development server
npm run dev

# Build for production
npm run build

# Generate API client with NSwag
# (Run after making API changes)
npx nswag run nswag.json
```

### Git Operations

```bash
# Stage all changes
git add -A

# Commit with message
git commit -m "description of changes"

# Push to remote
git push origin main

# Check status
git status
```

### Important Notes for Command Execution

1. **Use absolute paths** when running commands from different directories
2. **Always build before committing** - both backend and frontend must compile
3. **Check for errors** after each major operation
4. **Use `dotnet restore`** if dependencies are missing
5. **Clear build cache** if encountering strange errors: `dotnet clean && dotnet build`
6. **Restart Aspire** if the dashboard doesn't open: kill the process and run again

---

*Last Updated: Current Session*
