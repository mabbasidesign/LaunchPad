# LaunchPad API

## Overview
LaunchPad is a scalable, high-performance .NET API built with modern best practices. It includes dependency injection, rate limiting, distributed caching, structured logging, and service-oriented architecture.

## Features
- Asynchronous I/O and streaming
- Dependency injection with service interfaces
- Advanced rate limiting and throttling
- Distributed caching with Redis (IDistributedCache)
- Response compression
- Efficient serialization (System.Text.Json)
- Entity Framework Core with DbContext and repositories
- Structured logging with Serilog (console and daily rolling file, retention policy)
- Health checks and monitoring

## Project Structure
- `Controllers/` - API endpoints
- `Models/` - Domain entities and value objects
- `Services/` - Domain and application services
- `Data/` - DbContext and repository implementations
- `infra/` - Infrastructure as code (Bicep)

## DDD Practices (Planned)
The following Domain-Driven Design patterns are planned for future implementation:
- Rich domain models with business logic
- Repositories for aggregate persistence
- Domain services for cross-entity logic
- DTOs for API input/output
- Domain events for state changes
- Separation of domain, application, and infrastructure layers

## SQL Optimization (Planned)
The following SQL optimization strategies are planned for future implementation:
- Connection pooling
- Index tuning
- Stored procedures and batching
- Table partitioning
- Query pagination and AsNoTracking

## Middleware
- Rate limiting and throttling (10 requests per 10 seconds per IP)
- Response compression
- JWT Authentication with Bearer tokens
- Swagger/OpenAPI documentation
- Structured logging with Serilog

## API Endpoints

### Authentication
- **POST** `/api/register` - Register a new user
  - Request: `{ "username": "string", "password": "string" }`
  - Response: 200 OK with success message
  - Response: 400 Bad Request if username already exists

- **POST** `/api/auth/login` - Authenticate user and get JWT token
  - Request: `{ "username": "string", "password": "string" }`
  - Response: 200 OK with JWT token (1-hour expiration)
  - Response: 401 Unauthorized if credentials are invalid

### Books (All endpoints require JWT Authorization)
- **GET** `/api/v1.0/books` - Get all books
  - Response: 200 OK with list of BookDto objects

- **GET** `/api/v1.0/books/{id}` - Get book by ID
  - Response: 200 OK with BookDto or 404 Not Found

- **POST** `/api/v1.0/books` - Create a new book
  - Request: Book entity with Title, Author, ISBN, Price, Stock, Year
  - Response: 201 Created with BookDto

- **PUT** `/api/v1.0/books/{id}` - Update an existing book
  - Request: Book entity
  - Response: 204 No Content

- **DELETE** `/api/v1.0/books/{id}` - Delete a book
  - Response: 204 No Content or 404 Not Found

## Data Models

### User
```csharp
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }  // SHA256 hashed
}
```

### Book
```csharp
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string ISBN { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int Year { get; set; }
}
```

## Data Transfer Objects (DTOs)

### BookDto
```csharp
public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public int Year { get; set; }
}
```

### UserDto
```csharp
public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; }
}
```

### RegisterRequestDto / LoginRequestDto
```csharp
public class RegisterRequestDto
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class LoginRequestDto
{
    public string Username { get; set; }
    public string Password { get; set; }
}
```

## Authentication & Security

- **JWT Bearer Tokens**: Used for secure API access
  - Issuer: `LaunchPadAPI`
  - Audience: `LaunchPadUsers`
  - Expiration: 1 hour
  - Algorithm: HMAC-SHA256
  - Key validation enabled

- **Password Security**: Passwords are hashed using SHA256 before storage

- **Rate Limiting**: Global rate limiter restricts to 10 requests per 10 seconds per IP address with queue limit of 2

## API Versioning

- Current API version: `1.0`
- URL pattern: `/api/v{version}/[controller]`
- Default version: 1.0 (automatically assigned if not specified)

## Database

- **Database Engine**: SQL Server
- **ORM**: Entity Framework Core 8.0
- **Connection String**: Configured in `appsettings.json` under `DefaultConnection`
- **Migrations**: Located in `Migrations/` folder
  - Initial database schema with Books and Users tables
  - Column addition for Year in Book entity
  - User table creation with Id, Username, PasswordHash

### Seed Data
The database is initialized with sample books:
1. "The Pragmatic Programmer" by Andrew Hunt, David Thomas
2. "Clean Code" by Robert C. Martin
3. "Design Patterns" by Erich Gamma et al.

## Logging

- **Logger**: Serilog with structured logging
- **Sinks**:
  - Console output
  - Daily rolling file logs (retention: 30 days)
  - Location: `C:/LaunchpadLog/{DayName}/log-.txt`
- **Log Levels**: Information, Warning, Error
- **Structured Logging**: Used in BooksController for better debugging

## Getting Started
1. Clone the repository
2. Configure SQL credentials and Redis connection string in `appsettings.json`
3. Run migrations: `dotnet ef database update`
4. Start the API: `dotnet run`

## Deployment
- Use Bicep files in `infra/` for Azure resource provisioning
- Deploy with: `az deployment group create ...`
- Ensure Redis and logging storage are provisioned for production scenarios

## Azure Bicep Infrastructure

This project uses Azure Bicep files for infrastructure as code:

- `infra/resourceGroup.bicep`: Creates Azure resource groups in specified regions.
- `infra/main.bicep`: Entry point for deploying all Azure resources. Parameters include location, App Service, SQL Server, database, admin credentials, and SKU.
- `infra/appService.bicep`: Deploys Azure App Service Plan and Web App with managed identity and secure connection to Azure SQL.
- `infra/sqlDatabase.bicep`: Deploys Azure SQL Server and database with admin credentials, collation, and size settings.

### Typical Deployment Steps
1. Create resource group(s) with `az group create`.
2. Deploy resources using `az deployment group create` with `infra/main.bicep` and required parameters.
3. App Service connects securely to SQL Database using managed identity.

### Example Bicep Parameters
- `location`: Azure region (e.g., westus)
- `appServiceName`: Name for App Service
- `sqlServerName`: Name for SQL Server
- `sqlDbName`: Name for SQL Database
- `sqlAdminUser`: SQL admin username
- `sqlAdminPassword`: SQL admin password
- `appServiceSku`: App Service SKU (e.g., B1)

See the `infra/` folder for full Bicep templates and customization options.

## Project Files

- `LaunchPad.csproj` - Project file with NuGet dependencies
- `LaunchPad.sln` - Visual Studio solution file
- `LaunchPad.http` - HTTP request examples for testing endpoints
- `appsettings.json` - Application configuration (database, logging)
- `appsettings.Development.json` - Development-specific settings
- `Properties/launchSettings.json` - Launch profiles and URLs

## Technology Stack

- **Framework**: .NET 8.0 (C#)
- **Web Framework**: ASP.NET Core
- **Database**: SQL Server with Entity Framework Core 8.0
- **Authentication**: JWT Bearer Tokens
- **API Documentation**: Swagger/OpenAPI 6.6.2
- **Logging**: Serilog with file and console sinks
- **Rate Limiting**: Built-in ASP.NET Core Rate Limiting
- **Response Compression**: gzip
- **Caching**: Distributed Memory Cache
- **Infrastructure**: Azure Bicep for IaC

## Development

### Local HTTP Requests
See `LaunchPad.http` for example HTTP requests that can be executed directly in VS Code or Rider.

### Debugging
1. Build: `dotnet build`
2. Debug: `dotnet run`
3. Access Swagger UI: `https://localhost:5001/swagger`

## NuGet Packages

- Microsoft.AspNetCore.Authentication.JwtBearer
- Microsoft.AspNetCore.Mvc.Versioning
- Microsoft.AspNetCore.RateLimiting
- Microsoft.EntityFrameworkCore.SqlServer
- Serilog.AspNetCore
- Serilog.Sinks.File
- Swashbuckle.AspNetCore

## Contributing
Pull requests and issues are welcome. Please follow .NET best practices.

## License
MIT
