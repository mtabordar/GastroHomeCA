# GastroHomeCA

A Clean Architecture web application for product management and shopping trip tracking, built with ASP.NET Core Minimal APIs, React, and SQLite.

## Tech Stack

- **Backend:** ASP.NET Core 8.0, Minimal APIs, Entity Framework Core (SQLite), MediatR (CQRS), FluentValidation
- **Frontend:** React, Vite, NSwag (Auto-generated API client)
- **Architecture:** Clean Architecture / Onion Architecture
- **API Documentation:** Scalar (OpenAPI/Swagger)

## Getting Started

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

## Current Features

- **Product Management:** CRUD operations for products with barcode support
- **Todo Lists:** Task management with priority levels
- **Authentication:** ASP.NET Core Identity integration
- **API Documentation:** Scalar OpenAPI UI at `/scalar`

## References

- [Clean Architecture Website](https://cleanarchitecture.jasontaylor.dev)
- [Clean.Architecture.Solution.Template](https://github.com/jasontaylordev/CleanArchitecture)
- [ASP.NET Core Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/tutorials/minimal-apis)
- [MediatR Documentation](https://github.com/jasontaylordev/MediatR)
- [FluentValidation](https://fluentvalidation.net/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)

## Contributing

This CONTINUE.md file is automatically loaded by Continue when working with this project. You can:

1. Edit this file to add more specific guidance
2. Create additional rules.md files in subdirectories for component-specific documentation
3. Use Continue's AI features with full context of project patterns

For detailed architecture, design patterns, coding standards, and commands, see [Agents.md](Agents.md).

---

*Last Updated: Current Session*
