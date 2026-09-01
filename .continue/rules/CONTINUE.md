# GastroHomeCA - Project Guide

This guide provides comprehensive documentation for the GastroHomeCA project, built using Clean Architecture with ASP.NET Core Minimal APIs.

---

## 🏗️ Project Overview

**GastroHomeCA** is a web application built with Clean Architecture principles and the [Clean.Architecture.Solution.Template](https://github.com/jasontaylordev/CleanArchitecture).

**Key Technologies:**
- ASP.NET Core 8.0
- Clean Architecture / Onion Architecture
- Entity Framework Core (SQLite)
- MediatR (CQRS pattern)
- FluentValidation
- Minimal APIs with Auto-discovery

**Architecture Pattern:**
```
┌─────────────────────────────────────┐
│          Web (Presentation)          │
│   ┌──────────────────────────────┐  │
│   │   Infrastructure (Data, Ext)  │  │
│   ├──────────────────────────────┤  │
│   │   Application (Use Cases)     │  │
│   │   ┌──────────────────────┐   │  │
│   │   │    Domain (Pure)     │   │  │
│   │   └──────────────────────┘   │  │
│   └──────────────────────────────┘  │
└─────────────────────────────────────┘
```

---

## 📁 Project Structure

```
src/
├── Domain/                    # Pure domain logic (no dependencies)
│   ├── Entities/             # Domain entities (Product, TodoItem, etc.)
│   ├── Common/               # BaseEntity, shared concepts
│   ├── ValueObjects/         # Immutable value objects
│   ├── Events/               # Domain events
│   └── Constants/            # Global constants
│
├── Application/               # Use cases, commands, queries
│   ├── Common/
│   │   ├── DTOs/             # Data Transfer Objects
│   │   ├── Interfaces/       # IApplicationDbContext, ISender, ICurrentUser
│   │   └── Behaviours/       # MediatR pipeline behaviors
│   ├── [Feature]/
│   │   ├── Commands/         # Command definitions & handlers
│   │   ├── Queries/          # Query definitions & handlers
│   │   └── Mappings/         # Entity ↔ DTO mappings (static methods)
│   └── DependencyInjection.cs
│
├── Infrastructure/            # Data access, external services
│   ├── Data/                 # ApplicationDbContext, migrations
│   ├── Services/             # External service implementations
│   └── Extensions/           # Infrastructure extensions
│
├── Web/                       # API endpoints, presentation
│   ├── Endpoints/            # IEndpointGroup implementations
│   ├── Infrastructure/       # WebApplicationExtensions.MapEndpoints()
│   ├── Services/             # CurrentUser implementation
│   └── DependencyInjection.cs
│
├── AppHost/                   # Aspire hosting, composition root
│   └── DependencyInjection.cs
│
└── ServiceDefaults/           # Telemetry, logging, health checks
```

**Client Application:**
```
src/Web/ClientApp/             # React frontend
├── src/
│   ├── components/           # React components
│   ├── web-api-client.ts     # TypeScript API client (generated)
│   └── api-http-client.js    # HTTP client wrapper
```

---

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK
- Node.js 18+ (for the React client)
- Git

### Installation

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run the application
dotnet run --project src/AppHost
```

The Aspire dashboard will open automatically in your browser.

### Running the Client

```bash
cd src/Web/ClientApp
npm install
npm run dev
```

### Running Tests

```bash
# All tests
dotnet test

# Unit tests only
dotnet test tests/Application.UnitTests/

# Integration tests
dotnet test tests/Infrastructure.IntegrationTests/
```

---

## 📝 Development Workflow

### Coding Standards

#### DO:
- ✅ Use `init-only` properties for DTOs
- ✅ Keep domain entities pure (no external dependencies)
- ✅ Use static methods for entity ↔ DTO mappings
- ✅ Place validators in Application layer with FluentValidation
- ✅ Follow dependency direction (outer layers depend on inner)
- ✅ Use primary constructors for all new classes
- ✅ Use `int` for entity IDs (not `Guid`)
- ✅ Use `DateTime` (not `DateTimeOffset`)

#### DON'T:
- ❌ Expose domain entities directly as DTOs
- ❌ Mix infrastructure concerns with domain logic
- ❌ Use AutoMapper (use static method mappings)
- ❌ Put business rules in handlers (move to domain entities)
- ❌ Use `Guid` for IDs (project uses `int`)

### Build & Deployment

```bash
# Web API
dotnet build src/Web/

# React Client
cd src/Web/ClientApp
npm run build
```

---

## 🔑 Key Design Patterns

### 1. **Endpoint Groups (Minimal APIs)**

Endpoints are auto-discovered via `IEndpointGroup` interface.

**Example:**
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

**Example:**
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
}
```

### 4. **FluentValidation**

Validators use FluentValidation and are auto-registered.

**Example:**
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

**Example:**
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
}
```

---

## 📦 File Naming Conventions

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

## 🎯 Common Tasks

### Creating a New Feature

1. **Create Domain Entity** in `Domain/Entities/[Feature]/`
2. **Create Command** in `Application/[Feature]/Commands/Create/`
3. **Create Handler** in `Application/[Feature]/Handlers/Create/`
4. **Create Validator** in `Application/[Feature]/Commands/Create/`
5. **Create Mappings** in `Application/[Feature]/Mappings/`
6. **Create DTOs** in `Application/Common/DTOs/`
7. **Create Endpoint** in `Web/Endpoints/[Feature].cs`

### Scaffolding with Template

```bash
# Install template
dotnet new install Clean.Architecture.Solution.Template::10.8.0

# Create a new command
dotnet new ca-usecase --name CreateTodoList --feature-name TodoLists --usecase-type command --return-type int

# Create a new query
dotnet new ca-usecase -n GetTodos -fn TodoLists -ut query -rt TodosVm
```

### Adding a New Endpoint

```csharp
public class [Feature] : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost([MethodName]);
    }
}
```

### Database Migrations

```bash
cd src/Web/
dotnet ef migrations add [MigrationName]
dotnet ef database update
```

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

## 🧪 Testing Structure

| Test Type | Location | Purpose |
|-----------|----------|---------|
| Domain Unit Tests | `tests/Domain.UnitTests/` | Entity behavior, validation |
| Application Unit Tests | `tests/Application.UnitTests/` | Handler logic, validation |
| Integration Tests | `tests/Infrastructure.IntegrationTests/` | EF Core, DB operations |
| Acceptance Tests | `tests/Web.AcceptanceTests/` | E2E API flows |

---

## ⚠️ Important Notes

1. **ID Type:** All entities use `int Id` (NOT Guid)
2. **Date Format:** Use `DateTime` (not `DateTimeOffset`)
3. **Barcode Validation:** Pattern `^[0-9\s\-]*$`
4. **Route Defaults:** `/api/{EndpointGroupName}` unless overridden
5. **Validation Order:** Domain → FluentValidation → Handler
6. **Primary Constructors:** Use `class MyClass(int id)` instead of property-based initialization

---

## 🐛 Troubleshooting

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

**Cors errors:**
- Current config allows any origin
- Update for production: restrict allowed origins

---

## 📚 References

- [Clean Architecture Website](https://cleanarchitecture.jasontaylor.dev)
- [Clean.Architecture.Solution.Template](https://github.com/jasontaylordev/CleanArchitecture)
- [ASP.NET Core Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/tutorials/minimal-apis)
- [MediatR Documentation](https://github.com/jasontaylordev/MediatR)
- [FluentValidation](https://fluentvalidation.net/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)

---

## 📝 Notes for Contributors

This CONTINUE.md file is automatically loaded by Continue when working with this project. You can:

1. **Edit this file** to add more specific guidance
2. **Create additional rules.md files** in subdirectories for component-specific documentation
3. **Use Continue's AI features** with full context of project patterns

Remember to commit this file to share with your team!

---

*Last Updated: Current Session*
"