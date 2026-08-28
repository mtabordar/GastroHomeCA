# GastroHomeCA Architecture Guidelines

This document outlines the architecture patterns, folder structure, and coding standards for the GastroHomeCA project.

---

## 🏗️ Architecture Pattern

**Clean Architecture / Onion Architecture** with layered dependency direction pointing inward.

### Layer Responsibilities

| Layer | Responsibility | Allowed Dependencies |
|-------|---------------|---------------------|
| **Domain** | Pure business entities, value objects, domain events, domain-specific logic | None (no external dependencies) |
| **Application** | Use cases, commands, queries, DTOs, mappings, application services | Domain layer only |
| **Infrastructure** | Data access, external APIs, implementations of interfaces defined in other layers | Application + Domain layers |
| **Web/AppHost** | Presentation logic, Blazor Server app, API controllers, DI composition | All inner layers |
| **Shared** | Reusable utilities, extension methods, common types | None (or limited to domain) |
| **ServiceDefaults** | Telemetry, logging, health checks, background services | Infrastructure only |

---

## 📁 Folder Structure Guidelines

### Domain Layer (`src/Domain/`)
```
Domain/
├── [Feature]/              # Business entities per feature/module
│   ├── [Entity].cs        # Domain entities with business rules
│   ├── [ValueObject].cs   # Value objects
│   └── [Event].cs         # Domain events
├── Common/                # Shared domain concepts (BaseEntity, etc.)
└── Events/                # Domain event definitions
```

### Application Layer (`src/Application/`)
```
Application/
├── Common/
│   ├── DTOs/              # Data transfer objects for API/UI responses
│   └── Mappings/          # Entity ↔ DTO mappings
├── [Feature]/             # Business use cases per feature
│   ├── Commands/          # Command handlers, validators
│   ├── Queries/           # Query handlers
│   └── Events/            # Domain event handlers
└── DependencyInjection.cs # Application service registrations
```

### Infrastructure Layer (`src/Infrastructure/`)
```
Infrastructure/
├── [Feature]/             # Implementation per feature
│   ├── [Entity]Config.cs  # EF Core entity configurations (Fluent API)
│   └── [Service].cs       # External service implementations
├── Persistence/           # Database context, migrations
└── Common/                # Infrastructure shared utilities
```

### Shared Layer (`src/Shared/`)
```
Shared/
├── Extensions/            # Extension methods for DI, DbContext, etc.
├── Exceptions/            # Domain/application exceptions
└── Utilities/             # Reusable helpers (not feature-specific)
```

### ServiceDefaults Layer (`src/ServiceDefaults/`)
```
ServiceDefaults/
├── BackgroundServices.cs  # Timer-based background services
├── HealthChecks.cs        # Health check registrations
└── Extensions.cs          # Telemetry, logging, dependency injection
```

### AppHost Layer (`src/AppHost/`)
```
AppHost/
└── DependencyInjection.cs # Composition root: DI setup for all layers
```

---

## 📝 Coding Standards

### 1. **DTOs**
- ✅ Place in `Application/Common/DTOs/`
- ✅ Use immutable properties (`init-only`)
- ❌ Never expose domain entities directly as DTOs

**Example:**
```csharp
// src/Application/Application/Common/DTOs/TodoItemResponse.cs
public class TodoItemDto
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public string? Description { get; init; }
    public DateTime CreatedAt { get; init; }
}
```

### 2. **Mappings**
- ✅ Place in `Application/Common/Mappings/`
- ✅ Use static method-based mapping (no AutoMapper)
- ❌ Don't mix entity and DTO logic

**Example:**
```csharp
// src/Application/Application/Common/Mappings/TodoItemMappings.cs
public static class TodoItemMappings
{
    public static TodoItemDto ToDto(TodoItem entity) => new TodoItemDto
    {
        Id = entity.Id,
        Title = entity.Title,
        Description = entity.Description,
        CreatedAt = entity.CreatedAt
    };

    public static TodoItem ToEntity(TodoItemDto dto) => new TodoItem
    {
        Id = dto.Id,
        Title = dto.Title ?? string.Empty,
        Description = dto.Description,
        CreatedAt = dto.CreatedAt
    };
}
```

### 3. **Business Logic & Rules**
- ✅ Pure rules in Domain entities/value objects
- ✅ Use Cases in Application layer (commands/queries)
- ❌ Never mix infrastructure concerns with domain logic

**Domain Entity Example:**
```csharp
// src/Domain/TodoItems/TodoItem.cs
public class TodoItem : BaseEntity<TodoItem>
{
    public string Title { get; private set; }
    
    public void SetTitle(string title)
    {
        if (title.Length > 100)
            throw new InvalidOperationException("Title exceeds maximum length");
        
        this.Title = title; // Business rule enforced at setter
    }
}
```

### 4. **Fluent API Configurations**
- ✅ EF Core entity configurations in `Infrastructure/[Feature]/`
- ✅ Validators in Application layer (e.g., `[Feature]/Validators.cs`)
- ✅ Middleware/DI in AppHost/DependencyInjection.cs
- ✅ Extension methods in Shared/Extensions/

**EF Core Configuration Example:**
```csharp
// src/Infrastructure/TodoItems/TodoItemConfig.cs
public static class TodoItemConfig
{
    public static void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.ToTable("todo_items");
        
        builder.Property(e => e.Title).HasMaxLength(100);
        builder.Property(e => e.Description).HasMaxLength(500);
    }
}
```

### 5. **Domain Events**
- ✅ Define in `Domain/[Feature]/[Event].cs`
- ✅ Handle in `Application/[Feature]/Events/`
- ❌ Don't handle infrastructure-specific logic in event handlers

---

## 🔧 Dependency Injection Guidelines

### Application Layer DI (`Application/DependencyInjection.cs`)
```csharp
public static class ApplicationServiceDefaults
{
    public static void Configure(
        IServiceCollection services, 
        IServiceProvider provider)
    {
        // Register application services
        services.AddTransient<ICommandHandler<CreateTodoItemCommand>, CreateTodoItemHandler>();
        services.AddTransient<IQueryHandler<TodoItemsQuery>, TodoItemsQueryHandler>();
        
        // Add validators if using FluentValidation
        services.AddValidatorsFromAssemblyContaining<Application>()
                 .ConfigureExistingValidatorServices();
    }
}
```

### Infrastructure Layer DI (`Infrastructure/DependencyInjection.cs`)
```csharp
public static class InfrastructureServiceDefaults
{
    public static void Configure(
        IServiceCollection services, 
        IConfiguration configuration)
    {
        // Register data access
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
        // Add background services if needed
        services.AddHostedService<BackgroundCleanupService>();
    }
}
```

### AppHost Composition Root (`AppHost/DependencyInjection.cs`)
```csharp
public static class AppHost
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        services.AddApplication();
        services.AddInfrastructure();
        
        // Register web-specific services
        services.AddScoped<IUserService, UserService>();
        
        return services;
    }
}
```

---

## 🚫 Common Anti-Patterns to Avoid

| ❌ Anti-Pattern | ✅ Correct Approach |
|----------------|--------------------|
| Domain entities depending on EF Core | Use DTOs; load entities without tracking |
| Mappings in Infrastructure | Move mappings to Application layer |
| Business rules in application handlers | Encapsulate rules in domain entities |
| DTOs in Shared layer | DTOs are application-specific, not shared |
| Infrastructure calling domain events directly | Let application layer handle event dispatching |

---

## 📦 NuGet Package Guidelines

Refer to `Directory.Packages.props` for approved package versions. Common patterns:

- Domain: No external packages
- Application: FluentValidation, MediatR, etc.
- Infrastructure: EF Core, Npgsql/SQL Server drivers, Redis client, etc.
- All layers: Common shared libraries (if any)

---

## 🧪 Testing Guidelines

| Test Type | Layer Being Tested | Recommended Location |
|-----------|-------------------|---------------------|
| **Unit Tests** | Application logic | `tests/Application.UnitTests/` |
| **Domain Tests** | Domain entities/rules | `tests/Domain.UnitTests/` |
| **Integration Tests** | Infrastructure (DB, APIs) | `tests/Infrastructure.IntegrationTests/` |
| **Functional Tests** | Full application flow | `tests/Application.FunctionalTests/` |
| **Acceptance Tests** | UI/API end-to-end | `tests/Web.AcceptanceTests/` |

---

## 📝 File Naming Conventions

- **Entities:** `[Feature][Noun].cs` → `TodoItem.cs`, `UserAddress.cs`
- **Commands:** `Create[TodoItem]Command.cs` → `CreateTodoItemCommand.cs`
- **Queries:** `[Get/ListAll]TodoItemsQuery.cs` → `ListAllTodoItemsQuery.cs`
- **Handlers:** `[Action][Noun]Handler.cs` → `CreateTodoItemHandler.cs`
- **DTOs:** `ResponseDto`, `RequestDto`, `ViewModelDto` suffixes as appropriate
- **Mappings:** `[EntityName]Mappings.cs` → `TodoItemMappings.cs`

---

## 🔗 Version Control Notes

### Commit Message Format
```
feat: [layer] add feature description

Example:
feat: [domain] add todo item entity with validation rules
fix: [application] update mapping for response DTOs
refactor: [infrastructure] optimize EF Core query configuration
```

---

## 📚 References

- **Clean Architecture:** Robert C. Martin (Uncle Bob)
- **Onion Architecture:** Jeff Palermo
- **Domain-Driven Design:** Eric Evans
- **ASP.NET Aspire Patterns:** Microsoft documentation

---

## 🎯 Quick Reference Checklist

Before committing code, ask yourself:

- [ ] Are ALL `using` statements inside the namespace block?
- [ ] Is the entity placed in `Domain/Entities/` folder?
- [ ] Am I avoiding namespace/class name conflicts?
- [ ] Do I have appropriate DTOs for Create/Read/Update operations?
- [ ] Is the entity configuration in `Infrastructure/[Feature]/[Entity]Config.cs`?
- [ ] Am I using fully qualified names where needed to avoid ambiguity?
- [ ] Does my code compile without namespace resolution errors?

*Last Updated: [Current Date]*