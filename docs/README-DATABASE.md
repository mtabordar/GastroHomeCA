# GastroHomeCA - Database Documentation

## Overview
SQLite database for GastroHomeCA, managed through Entity Framework Core migrations.

## Technology Stack

- **Database:** SQLite
- **ORM:** Entity Framework Core 8.0
- **Migration Tool:** EF Core CLI
- **Connection String:** Configured in `appsettings.json`

## Database Configuration

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
}
```

## Entity Configurations

### Product Configuration
Located in `src/Infrastructure/Data/Configurations/ProductConfiguration.cs`:
- `Name`: required, max 200 chars
- `Category`: required, max 100 chars
- `Barcode`: max 50 chars, unique index
- `CurrentPrice`: decimal(18,2)

## Database Interceptors

### AuditableEntityInterceptor
Located in `src/Infrastructure/Data/Interceptors/AuditableEntityInterceptor.cs`
- Automatically sets `CreatedBy`, `CreatedDate`, `LastUpdatedBy`, `LastUpdatedDate` on `BaseAuditableEntity` entities.

### DispatchDomainEventsInterceptor
Located in `src/Infrastructure/Data/Interceptors/DispatchDomainEventsInterceptor.cs`
- Dispatches domain events via MediatR before saving changes.

## Migrations

### Adding Migrations
```bash
cd src/Web/
dotnet ef migrations add [MigrationName]
```

### Updating Database
```bash
dotnet ef database update
```

### Other Commands
```bash
dotnet ef migrations remove          # Remove last migration
dotnet ef migrations list            # List migrations
dotnet ef migrations script [From] [To]  # Generate SQL script
```

## Database Initialization

Database is initialized automatically in Development environment:
```csharp
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}
```

Seed data can be added in `ApplicationDbContextInitialiser.cs`.

## Database File Location

The SQLite database file is located at:
```
src/Web/Data/GastroHomeCA.db
```

Additional SQLite files:
```
src/Web/Data/GastroHomeCA.db-shm  # Shared memory file
src/Web/Data/GastroHomeCA.db-wal  # Write-ahead log file
```

## Entity Relationships

### Product
- No direct relationships currently defined
- Future: One-to-many with PriceHistory

## Indexes

| Entity | Column | Type |
|--------|--------|------|
| Product | Id | Primary key |
| Product | Barcode | Unique index |

## Data Types

| Column | Type | Notes |
|--------|------|-------|
| Id | INTEGER | Primary key, auto-increment |
| Name | TEXT | Max length 200 |
| Category | TEXT | Max length 100 |
| Barcode | TEXT | Max length 50, nullable |
| CurrentPrice | DECIMAL(18,2) | Price in currency |
| CreatedDate | DATETIME | Auto-set on creation |
| LastUpdatedDate | DATETIME | Auto-set on update |
| CreatedBy | TEXT | Nullable user name |
| LastUpdatedBy | TEXT | Nullable user name |

## Best Practices

### DO:
- Use migrations for schema changes
- Keep migrations focused (one change per migration)
- Test migrations on a copy of production data
- Use decimal(18,2) for monetary values
- Add indexes on frequently queried columns

### DON'T:
- Manually edit the database file
- Skip migrations in production
- Use string type for monetary values
- Forget to add indexes on foreign keys
- Mix migration styles (code-first vs database-first)

## Backup & Restore

### Backup
```bash
cp src/Web/Data/GastroHomeCA.db src/Web/Data/GastroHomeCA.db.backup
```

### Restore
```bash
cp src/Web/Data/GastroHomeCA.db.backup src/Web/Data/GastroHomeCA.db
```

## Future Enhancements

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
