# GastroHomeCA

A Clean Architecture web application for product management and shopping trip tracking, built with ASP.NET Core Minimal APIs, React, and SQLite.

## 🏗️ Architecture

This project follows **Clean Architecture** (Onion Architecture) principles using the [Clean.Architecture.Solution.Template](https://github.com/jasontaylordev/CleanArchitecture):

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

**Tech Stack:**
- **Backend:** ASP.NET Core 8.0, Minimal APIs, Entity Framework Core (SQLite), MediatR (CQRS), FluentValidation
- **Frontend:** React, Vite, NSwag (Auto-generated API client)
- **Architecture:** Clean Architecture / Onion Architecture
- **API Documentation:** Scalar (OpenAPI/Swagger)

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

## 📝 Development Workflow

### Build Commands

```bash
# Build entire solution
dotnet build

# Build specific project
dotnet build src/Web/

# Build React client
cd src/Web/ClientApp && npm run build
```

**⚠️ Always build before committing!** The project requires both backend and frontend to build successfully.

### Running Tests

```bash
# All tests
dotnet test

# Unit tests only
dotnet test tests/Application.UnitTests/

# Integration tests
dotnet test tests/Infrastructure.IntegrationTests/

# Acceptance tests
dotnet test tests/Web.AcceptanceTests/
```

### Database Migrations

```bash
cd src/Web/
dotnet ef migrations add [MigrationName]
dotnet ef database update
```

## 📁 Project Structure

```
src/
├── Domain/                    # Pure domain logic (no dependencies)
│   ├── Entities/             # Product, TodoItem, TodoList
│   ├── Common/               # BaseEntity, shared concepts
│   ├── ValueObjects/         # Immutable value objects
│   ├── Events/               # Domain events
│   └── Constants/            # Global constants
│
├── Application/               # Use cases, commands, queries
│   ├── Common/
│   │   ├── DTOs/             # Data Transfer Objects
│   │   ├── Interfaces/       # IApplicationDbContext, ISender
│   │   └── Behaviours/       # MediatR pipeline behaviors
│   ├── Product/              # Product feature
│   │   ├── Commands/         # Create, Update commands
│   │   ├── Queries/          # GetProduct, ListProducts
│   │   └── Mappings/         # Entity ↔ DTO mappings
│   └── TodoLists/            # TodoLists feature
│   └── TodoItems/            # TodoItems feature
│
├── Infrastructure/            # Data access, external services
│   ├── Data/                 # ApplicationDbContext, migrations
│   ├── Services/             # External service implementations
│   └── Identity/             # ASP.NET Core Identity
│
├── Web/                       # API endpoints, presentation
│   ├── Endpoints/            # Products, TodoItems, TodoLists, Users
│   ├── ClientApp/            # React frontend
│   ├── Services/             # CurrentUser implementation
│   └── Infrastructure/       # WebApplicationExtensions
│
├── AppHost/                   # Aspire hosting
└── ServiceDefaults/           # Telemetry, logging
```

## 🔑 Key Design Patterns

### 1. Endpoint Groups (Minimal APIs)

Endpoints are auto-discovered via `IEndpointGroup` interface:

```csharp
public class Products : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(CreateProduct);
        groupBuilder.MapGet("/{id:int}", GetProduct);
    }
}
```

### 2. Command Pattern (MediatR)

```csharp
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
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

### 3. Domain Validation

Business rules live in domain entities:

```csharp
public void Create(string name, string category, string? barcode = null, decimal price = 0m)
{
    if (string.IsNullOrWhiteSpace(name)) 
        throw new ArgumentException("Product name cannot be empty.");
    // ...
}
```

## 📦 Coding Standards

### DO:
- ✅ Use `init-only` properties for DTOs
- ✅ Keep domain entities pure (no external dependencies)
- ✅ Use static methods for entity ↔ DTO mappings
- ✅ Use primary constructors for all new classes
- ✅ Use `int` for entity IDs (not `Guid`)
- ✅ Use `DateTime` (not `DateTimeOffset`)

### DON'T:
- ❌ Expose domain entities directly as DTOs
- ❌ Mix infrastructure concerns with domain logic
- ❌ Use AutoMapper (use static method mappings)
- ❌ Put business rules in handlers (move to domain entities)
- ❌ Use `Guid` for IDs (project uses `int`)

## 🎯 Current Features

- **Product Management:** CRUD operations for products with barcode support
- **Todo Lists:** Task management with priority levels
- **Authentication:** ASP.NET Core Identity integration
- **API Documentation:** Scalar OpenAPI UI at `/scalar`

## 🧪 Testing

| Test Type | Location | Purpose |
|-----------|----------|---------|
| Domain Unit Tests | `tests/Domain.UnitTests/` | Entity behavior, validation |
| Application Unit Tests | `tests/Application.UnitTests/` | Handler logic, validation |
| Integration Tests | `tests/Infrastructure.IntegrationTests/` | EF Core, DB operations |
| Acceptance Tests | `tests/Web.AcceptanceTests/` | E2E API flows |

## 📚 References

- [Clean Architecture Website](https://cleanarchitecture.jasontaylor.dev)
- [Clean.Architecture.Solution.Template](https://github.com/jasontaylordev/CleanArchitecture)
- [ASP.NET Core Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/tutorials/minimal-apis)
- [MediatR Documentation](https://github.com/jasontaylordev/MediatR)
- [FluentValidation](https://fluentvalidation.net/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)

## 🤝 Contributing

This CONTINUE.md file is automatically loaded by Continue when working with this project. You can:

1. Edit this file to add more specific guidance
2. Create additional rules.md files in subdirectories for component-specific documentation
3. Use Continue's AI features with full context of project patterns

Remember to commit this file to share with your team!

---

*Last Updated: Current Session*
