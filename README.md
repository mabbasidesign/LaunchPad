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
- **CORS** (Cross-Origin Resource Sharing) - Allows frontend applications to access the API
- JWT Authentication with Bearer tokens
- **Global Exception Handling** - Centralized error handling with structured error responses
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

## CORS (Cross-Origin Resource Sharing)

The API is configured to accept requests from whitelisted frontend origins, enabling secure cross-origin communication.

### Allowed Origins
- `http://localhost:3000` - React development server
- `http://localhost:5000` - Local development (HTTP)
- `http://localhost:5173` - Vite development server
- `https://localhost:3001` - Local development (HTTPS)

### CORS Policy Configuration
- **Methods**: All HTTP methods allowed (GET, POST, PUT, DELETE, OPTIONS)
- **Headers**: All headers allowed in requests
- **Credentials**: Enabled (allows cookies and authorization headers to be sent)
- **Exposed Headers**: `X-Pagination` (custom headers exposed to client)

### How to Add More Origins
Edit `Program.cs` in the CORS configuration:
```csharp
options.WithOrigins(
    "http://localhost:3000",
    "https://yourdomain.com"  // Add production domain here
)
```

### CORS Errors
If you get a CORS error in the browser console:
1. Check that your frontend origin is in the `WithOrigins()` list
2. Ensure you're using the correct protocol (http vs https)
3. Verify port number matches
4. Clear browser cache or use incognito mode

## Exception Handling

The application uses **Global Exception Handling Middleware** to catch and handle all unhandled exceptions with consistent, structured error responses.

### Handled Exception Types
- **ArgumentNullException / ArgumentException**: Returns HTTP 400 Bad Request
- **InvalidOperationException**: Returns HTTP 400 Bad Request
- **KeyNotFoundException**: Returns HTTP 404 Not Found
- **UnauthorizedAccessException**: Returns HTTP 401 Unauthorized
- **Unhandled Exceptions**: Returns HTTP 500 Internal Server Error

### Exception Response Format
All exception responses follow this standard format:
```json
{
  "message": "An internal server error occurred.",
  "details": "Exception message details",
  "traceId": "0HMVJR0T7JIUG:00000001",
  "timestamp": "2026-02-15T14:30:00Z"
}
```

### Features
- **Centralized Handling**: All exceptions caught in one middleware
- **Structured Logging**: Full exception details logged with Serilog
- **Consistent Responses**: Client receives standardized error format
- **Request Context**: Logs include request path, method, and trace ID
- **Security**: Sensitive stack traces logged internally, generic messages sent to clients

### Implementation
- Middleware: `Middleware/ExceptionHandlingMiddleware.cs`
- Extension: `UseExceptionHandling()` registered in `Program.cs`
- Executed first in the middleware pipeline to catch all exceptions

## Input Validation

All API endpoints use **Data Annotations** for input validation. Invalid requests are rejected with HTTP 400 Bad Request and detailed error messages.

### Book Validation
- **Title**: Required, 1-200 characters
- **Author**: Required, 1-200 characters
- **ISBN**: Optional, 10-20 characters, valid ISBN format
- **Price**: Required, 0.01-10000
- **Stock**: Required, 0-100000
- **Year**: Required, 1000-2100

### User Validation
- **Username**: Required, 3-50 characters, alphanumeric + underscore/hyphen only
- **PasswordHash**: Required (auto-generated)

### Registration (RegisterRequestDto) Validation
- **Username**: Required, 3-50 characters, alphanumeric + underscore/hyphen only
- **Password**: Enforced through `PasswordComplexityAttribute`
  - Length: 8-128 characters
  - Must contain lowercase letter (a-z)
  - Must contain uppercase letter (A-Z)
  - Must contain digit (0-9)
  - Must contain special character (@$!%*?&)
  - Cannot contain spaces
  - Cannot have more than 2 consecutive identical characters

### Login (LoginRequestDto) Validation
- **Username**: Required, 3-50 characters
- **Password**: Required, 8-128 characters

### Validation Error Response Example
```json
{
  "errors": {
    "Title": ["Title is required."],
    "Price": ["Price must be between 0.01 and 10000."]
  },
  "type": "https://tools.ietf.org/html/rfc7231#section-6.4.1",
  "title": "One or more validation errors occurred.",
  "status": 400
}
```

## Password Complexity Validation

The API enforces strict password complexity requirements through a custom `PasswordComplexityAttribute` validation attribute for enhanced security.

### Password Requirements

All passwords must meet these criteria:
- **Length**: 8-128 characters
- **Lowercase Letter**: At least one (a-z)
- **Uppercase Letter**: At least one (A-Z)
- **Digit**: At least one (0-9)
- **Special Character**: At least one (@, $, !, %, *, ?, &)
- **No Spaces**: Whitespace characters not allowed
- **No Repetition**: Cannot have more than 2 consecutive identical characters (e.g., "aaa" fails)

### Valid Password Examples
- `MyPassword123!` - Contains all required character types
- `SecureP@ssw0rd` - Strong password with special character at position 7
- `Abc123$xyz` - Simple but meets all requirements
- `Test@Pass99` - Special character in middle of password

### Invalid Password Examples

| Password | Reason |
|----------|--------|
| `password123` | Missing uppercase letter and special character |
| `PASSWORD!` | Missing lowercase letter and digit |
| `Pass123` | Missing special character |
| `Pass 123!` | Contains space character |
| `Passsss1!` | More than 2 consecutive 's' characters |
| `P@1` | Too short (less than 8 characters) |
| `MyPassword` | No digit or special character |

### Custom Validator Implementation

- **Location**: `Validation/PasswordComplexityAttribute.cs`
- **Usage**: Applied to `RegisterRequestDto.Password` in `DTO/UserDto.cs`
- **Features**:
  - Provides detailed error messages for each validation rule
  - Checks character types individually
  - Detects consecutive identical characters
  - Fully reusable across any password property
  - Activates on model validation during registration

### Password Validation Error Response

When password validation fails, the API returns an HTTP 400 response with specific error details:

```json
{
  "errors": {
    "Password": ["Password must contain at least one special character (@, $, !, %, *, ?, or &)."]
  },
  "type": "https://tools.ietf.org/html/rfc7231#section-6.4.1",
  "title": "One or more validation errors occurred.",
  "status": 400
}
```

### Security Benefits

- Prevents weak passwords from being created
- Reduces risk of account compromise
- Enforces industry-standard password complexity rules
- Provides immediate feedback to users on password requirements
- Helps meet security compliance standards (ISO 27001, NIST, etc.)

## Logging

- **Logger**: Serilog with structured logging
- **Sinks**:
  - Console output
  - Daily rolling file logs (retention: 30 days)
  - Location: `C:/LaunchpadLog/{DayName}/log-.txt`
- **Log Levels**: Information, Warning, Error
- **Structured Logging**: Used throughout for better debugging

### Controller Logging

Each controller action logs:
- **Request Start**: Action name, request parameters, and authenticated user
- **Validation**: All validation errors with field details
- **Business Logic**: Resource not found, duplicates, constraint failures
- **Success**: Completion with execution time and affected resource IDs
- **Errors**: Full exception details for debugging

### Log Format
```
[ACTION_NAME] Message. Key: {Value}. Duration: {DurationMs}ms. User: {Username}
```

### Examples
- `[LOGIN] Request started. Username: john_doe`
- `[GET_BOOKS] Retrieved 10 books. Duration: 42ms. User: john_doe`
- `[CREATE_BOOK] Failed to create book. Duration: 150ms. User: john_doe`
- `[UPDATE_BOOK] Validation failed for BookId 5. Duration: 12ms. User: john_doe`
- `[DELETE_BOOK] Book not found. BookId: 999. Duration: 8ms. User: john_doe`

### Performance Tracking

All controller actions track execution time using `Stopwatch`:
- Request start and end times captured
- Duration (in milliseconds) logged for every operation
- Helps identify slow endpoints and performance bottlenecks
- Useful for load testing and optimization

### Events Logged
  - User registration and login attempts with success/failure reasons
  - Book CRUD operations with resource details
  - Data validation failures with error messages
  - Authorization failures
  - Exception details with stack traces

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
