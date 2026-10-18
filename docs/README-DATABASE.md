# GastroHomeCA - Database Documentation

## Overview
SQLite database for GastroHomeCA, managed through Entity Framework Core migrations.

---

## 1. Technology Stack

- **Database:** SQLite
- **ORM:** Entity Framework Core 8.0
- **Migration Tool:** EF Core CLI
- **Connection String:** Configured in `appsettings.json`

---

## 2. Database Configuration

### Connection String
Located in `src/Web/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=GastroHomeCA.db"
  }
}
```

### ApplicationDbContext
Located in `src/Infrastructure/Data/ApplicationDbContext.cs`:

```csharp
public class ApplicationDbContext : BaseAuditableDbContext, IApplicationDbContext
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    public DbSet<TodoList> TodoLists => Set<TodoList>();
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        ApplyConfigurations(builder);
        base.OnModelCreating(builder);
    }
}
```

---

## 3. Entity Configurations

### Product Configuration
Located in `src/Infrastructure/Data/Configurations/ProductConfiguration.cs`:

```csharp
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Category).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Barcode).HasMaxLength(50);
        builder.Property(p => p.CurrentPrice).HasColumnType("decimal(18,2)");
        
        builder.HasIndex(p => p.Barcode).IsUnique();
    }
}
```

### TodoItem Configuration
Located in `src/Infrastructure/Data/Configurations/TodoItemConfiguration.cs`:

```csharp
public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Title).IsRequired().HasMaxLength(200);
        builder.Property(t => p.Done).IsRequired();
        builder.Property(t => p.Priority).HasConversion<int>();
    }
}
```

### TodoList Configuration
Located in `src/Infrastructure/Data/Configurations/TodoListConfiguration.cs`:

```csharp
public class TodoListConfiguration : IEntityTypeConfiguration<TodoList>
{
    public void Configure(EntityTypeBuilder<TodoList> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Title).IsRequired().HasMaxLength(200);
        builder.Property(t => p.Colour).HasColumnType("text");
    }
}
```

---

## 4. Domain Entities

### Product Entity
Located in `src/Domain/Entities/Product.cs`:

```csharp
public class Product : BaseAuditableEntity
{
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string? Barcode { get; init; }
    public decimal CurrentPrice { get; init; }
    
    public void Create(string name, string category, string? barcode = null, decimal price = 0m)
    {
        if (string.IsNullOrWhiteSpace(name)) 
            throw new ArgumentException("Product name cannot be empty.");
        if (string.IsNullOrWhiteSpace(category)) 
            throw new ArgumentException("Category cannot be empty.");
        
        Name = name;
        Category = category;
        Barcode = barcode;
        CurrentPrice = price;
    }
    
    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0) 
            throw new ArgumentException("Price cannot be negative.");
        CurrentPrice = newPrice;
    }
}
```

### TodoItem Entity
Located in `src/Domain/Entities/TodoItem.cs`:

```csharp
public class TodoItem : BaseAuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public bool Done { get; set; }
    public int Priority { get; set; }
    public int ListId { get; set; }
    public TodoList? List { get; set; }
}
```

### TodoList Entity
Located in `src/Domain/Entities/TodoList.cs`:

```csharp
public class TodoList : BaseAuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string Colour { get; set; } = Colour.White.Value;
    public IList<TodoItem> Items { get; } = new List<TodoItem>();
}
```

---

## 5. Base Entities

### BaseEntity
Located in `src/Domain/Common/BaseEntity.cs`:

```csharp
public abstract class BaseEntity
{
    public int Id { get; init; }
}
```

### BaseAuditableEntity
Located in `src/Domain/Common/BaseAuditableEntity.cs`:

```csharp
public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastUpdatedDate { get; set; }
    public string? LastUpdatedBy { get; set; }
}
```

---

## 6. Database Interceptors

### AuditableEntityInterceptor
Located in `src/Infrastructure/Data/Interceptors/AuditableEntityInterceptor.cs`:

Sets CreatedBy, CreatedDate, LastUpdatedBy, and LastUpdatedDate automatically:

```csharp
public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUser _currentUser;
    
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }
    
    private void UpdateAuditableEntities(DbContext? context)
    {
        if (context == null) return;
        
        foreach (var entry in context.ChangeTracker.Entries<BaseAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedDate = DateTime.Now;
                entry.Entity.CreatedBy = _currentUser.UserName;
            }
            
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.LastUpdatedDate = DateTime.Now;
                entry.Entity.LastUpdatedBy = _currentUser.UserName;
            }
        }
    }
}
```

### DispatchDomainEventsInterceptor
Located in `src/Infrastructure/Data/Interceptors/DispatchDomainEventsInterceptor.cs`:

Dispatches domain events before saving changes:

```csharp
public class DispatchDomainEventsInterceptor : SaveChangesInterceptor
{
    private readonly IMediator _mediator;
    
    public override async Task<int> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        await DispatchDomainEventsAsync(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken).ConfigureAwait(false);
    }
    
    private async Task DispatchDomainEventsAsync(DbContext? context)
    {
        if (context == null) return;
        
        var entities = context.ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .ToList();
        
        var domainEvents = entities.SelectMany(e => e.Entity.DomainEvents).ToList();
        
        entities.ForEach(e => e.Entity.ClearDomainEvents());
        
        foreach (var domainEvent in domainEvents)
            await _mediator.Publish(domainEvent, cancellationToken);
    }
}
```

---

## 7. Migrations

### Adding Migrations
```bash
cd src/Web/
dotnet ef migrations add [MigrationName]
```

### Updating Database
```bash
dotnet ef database update
```

### Removing Last Migration
```bash
dotnet ef migrations remove
```

### Listing Migrations
```bash
dotnet ef migrations list
```

### Generating SQL Script
```bash
dotnet ef migrations script [FromMigration] [ToMigration]
```

---

## 8. Database Initialization

Database is initialized automatically in Development environment:

```csharp
// In Program.cs
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}
```

### Initial Data
Seed data can be added in `ApplicationDbContextInitialiser.cs`:

```csharp
public static async Task SeedExampleDataAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    await dbContext.Database.EnsureCreatedAsync();
    
    // Seed products
    if (!dbContext.Products.Any())
    {
        dbContext.Products.AddRange(
            new Product { Name = "Product 1", Category = "Category 1", CurrentPrice = 10.00m }
        );
        await dbContext.SaveChangesAsync();
    }
}
```

---

## 9. Database File Location

The SQLite database file is located at:
```
src/Web/Data/GastroHomeCA.db
```

Additional SQLite files:
```
src/Web/Data/GastroHomeCA.db-shm  # Shared memory file
src/Web/Data/GastroHomeCA.db-wal  # Write-ahead log file
```

---

## 10. Entity Relationships

### Product Relationships
- **Product** (many) → **PriceHistory** (one) - One-to-many (future)
- No direct relationships currently defined

### TodoItem Relationships
- **TodoItem** (many) → **TodoList** (one) - Many-to-one
- Foreign key: `ListId`

### TodoList Relationships
- **TodoList** (one) → **TodoItem** (many) - One-to-many
- Navigation property: `Items`

---

## 11. Indexes

### Product Indexes
- `Barcode` - Unique index
- Primary key on `Id`

### TodoItem Indexes
- Primary key on `Id`
- Index on `ListId` (foreign key)

### TodoList Indexes
- Primary key on `Id`

---

## 12. Data Types

| Column | Type | Notes |
|--------|------|-------|
| Id | INTEGER | Primary key, auto-increment |
| Name | TEXT | Max length 200 |
| Category | TEXT | Max length 100 |
| Barcode | TEXT | Max length 50, nullable |
| CurrentPrice | DECIMAL(18,2) | Price in currency |
| Title | TEXT | Max length 200 |
| Done | INTEGER | Boolean (0 or 1) |
| Priority | INTEGER | Priority level |
| ListId | INTEGER | Foreign key |
| Colour | TEXT | Color value |
| CreatedDate | DATETIME | Auto-set on creation |
| LastUpdatedDate | DATETIME | Auto-set on update |
| CreatedBy | TEXT | Nullable user name |
| LastUpdatedBy | TEXT | Nullable user name |

---

## 13. Best Practices

### ✅ DO:
- Use migrations for schema changes
- Keep migrations focused (one change per migration)
- Test migrations on a copy of production data
- Use decimal(18,2) for monetary values
- Add indexes on frequently queried columns

### ❌ DON'T:
- Manually edit the database file
- Skip migrations in production
- Use string type for monetary values
- Forget to add indexes on foreign keys
- Mix migration styles (code-first vs database-first)

---

## 14. Backup & Restore

### Backup
```bash
cp src/Web/Data/GastroHomeCA.db src/Web/Data/GastroHomeCA.db.backup
```

### Restore
```bash
cp src/Web/Data/GastroHomeCA.db.backup src/Web/Data/GastroHomeCA.db
```

---

## 15. Future Enhancements

- [ ] Add PriceHistory entity
- [ ] Add ShoppingTrip entity
- [ ] Add PurchasedItem entity
- [ ] Implement soft deletes
- [ ] Add database auditing table
- [ ] Implement database partitioning for large tables
- [ ] Add database monitoring and metrics
- [ ] Implement database backup automation

---

*Last Updated: Current Session*
