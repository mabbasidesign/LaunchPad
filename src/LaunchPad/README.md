# LaunchPad API

## Overview
LaunchPad is a scalable, high-performance .NET API built with modern best practices. It includes dependency injection, rate limiting, distributed caching, structured logging, and service-oriented architecture.

## Features
- Asynchronous I/O and streaming
- Dependency injection with service interfaces
- **CQRS Pattern** with MediatR for command/query segregation
- Advanced rate limiting and throttling
- Distributed caching with Redis (IDistributedCache)
- Response compression
- Efficient serialization (System.Text.Json)
- Entity Framework Core with DbContext and repositories
- Structured logging with Serilog (console and daily rolling file, retention policy)
- Health checks and monitoring
- JWT Authentication with Bearer tokens
- Custom validation attributes (password complexity)
- Orders with totals, tax, discounts, and aggregation queries

## Project Structure

```
LaunchPad/                          (Git repository root)
├── LaunchPad.sln                  (Single solution file for all projects)
│
├── src/LaunchPad/                 (Main application)
│   ├── Controllers/               - API endpoints (thin controllers using MediatR)
│   ├── Models/                    - Domain entities and value objects
│   ├── Services/                  - Domain and application services
│   ├── Data/                      - DbContext and repository implementations
│   ├── Features/                  - CQRS Commands and Queries with handlers
│   │   ├── Auth/Commands/         - LoginCommand, RegisterCommand with handlers
│   │   ├── Books/Commands/        - Create, Update, Delete commands with handlers
│   │   └── Books/Queries/         - GetAllBooks, GetBookById queries with handlers
│   ├── DTO/                       - Data Transfer Objects
│   ├── Middleware/                - Custom middleware (exception handling)
│   ├── Validation/                - Custom validation attributes
│   ├── Program.cs
│   └── LaunchPad.csproj
│
└── tests/LaunchPad.Tests/         (Unit tests)
    ├── Handlers/                  - MediatR handler tests
    │   ├── LoginCommandHandlerTests.cs
    │   ├── GetAllBooksQueryHandlerTests.cs
    │   └── CreateBookCommandHandlerTests.cs
    └── LaunchPad.Tests.csproj

```

## Architecture: CQRS with MediatR

LaunchPad implements **CQRS (Command Query Responsibility Segregation)** pattern using **MediatR**, enabling clean separation of read and write operations.

### How It Works

**Request Flow:**
```
Controller (HTTP) 
  ↓
IMediator.Send(Command/Query)
  ↓
MediatR Routes to Appropriate Handler
  ↓
Handler Executes Business Logic (calls Services/Repositories)
  ↓
Handler Returns Result
  ↓
Controller Returns HTTP Response
```

### Commands (Write Operations)

Commands modify data and return `Unit` (void) or an entity result.

**Authentication Commands:**
- `LoginCommand` → `LoginCommandHandler` → Validates credentials, generates JWT token
- `RegisterCommand` → `RegisterCommandHandler` → Creates new user

**Book Commands:**
- `CreateBookCommand` → `CreateBookCommandHandler` → Adds book to database
- `UpdateBookCommand` → `UpdateBookCommandHandler` → Modifies existing book
- `DeleteBookCommand` → `DeleteBookCommandHandler` → Removes book from database

### Queries (Read Operations)

Queries retrieve data without modifying state and return DTOs or collections.

**Book Queries:**
- `GetAllBooksQuery` → `GetAllBooksQueryHandler` → Returns all books
- `GetBookByIdQuery` → `GetBookByIdQueryHandler` → Returns single book by ID

**Order Queries:**
- Orders use service-based queries for summary and top items aggregation

### Benefits

✅ **Separation of Concerns**: Controllers only handle HTTP, handlers handle business logic  
✅ **Loose Coupling**: Controllers depend on IMediator, not on services  
✅ **Improved Testability**: Handlers can be tested independently  
✅ **Reusability**: Handlers can be invoked from controllers, console apps, background jobs, webhooks  
✅ **Scalability**: Read and write operations can be optimized independently  
✅ **Maintainability**: Clear structure with explicit commands and queries  

### Performance Monitoring

All handlers include `Stopwatch` logging to track execution duration:
```
[COMMAND: Login] User logged in successfully. UserID: 1, Username: Admin, Duration: 943ms
```

## DDD & Architecture Patterns

### Implemented Patterns
✅ **CQRS (Command Query Responsibility Segregation)** - Separate read and write operation handlers  
✅ **Repository Pattern** - Data access abstraction layer for Books and Users  
✅ **Dependency Injection** - Loose coupling with interface-based design  
✅ **Data Transfer Objects (DTOs)** - API input/output decoupling from domain models  
✅ **Service Layer** - Business logic separated from controllers  
✅ **MediatR Pipeline** - Centralized request handling and cross-cutting concerns

### Planned Enhancements
- Rich domain models with business logic embedded
- Domain services for cross-entity operations
- Domain events for state change notifications
- Aggregate roots with bounded contexts
- Value objects for domain primitives

## SQL Optimization

The following SQL optimization strategies have been implemented:
- **Database Indexes** - Unique indexes on `Book.Id` and `User.Username` for faster lookups
- **AsNoTracking()** - Read-only queries disable change tracking to reduce memory overhead
- **ExecuteDeleteAsync()** - Direct database deletion without loading entities, eliminating unnecessary round-trips
- **ExistsAsync()** - Lightweight existence checks using `AnyAsync()` instead of loading full entities
- **Distributed Caching** - Redis cache for book queries (5-minute expiration) with automatic invalidation on writes
- **Parameterized Queries** - All queries use EF Core LINQ (no raw SQL) to prevent SQL injection
- **Pagination** - Query limits with `Skip()` and `Take()` for efficient large result sets

### Performance Improvements
- Reduced database round-trips through efficient query patterns
- Decreased memory allocation by avoiding unnecessary entity tracking
- Faster authentication checks with index on username
- Improved response times on read-heavy operations through distributed caching

## Pagination

Pagination support is built into the Books API for efficient handling of large datasets:

### Implementation
- **Query-based**: Uses `Skip()` and `Take()` for server-side pagination
- **Generic DTO**: `PaginatedResultDto<T>` contains both data and metadata
- **Computed Properties**: `TotalPages`, `HasPreviousPage`, `HasNextPage` calculated automatically
- **Default Parameters**: `pageNumber=1`, `pageSize=10` for safe defaults

### Usage Example
```
GET /api/v1.0/books?pageNumber=2&pageSize=20
```

### Response Structure
```json
{
  "items": [{...book data...}],
  "pageNumber": 2,
  "pageSize": 20,
  "totalCount": 150,
  "totalPages": 8,
  "hasPreviousPage": true,
  "hasNextPage": true
}
```

### Benefits
✅ Reduces memory overhead on large datasets  
✅ Faster page loads with smaller result sets  
✅ Client-friendly pagination metadata  
✅ Efficient database queries with automatic limiting  

## Testing

LaunchPad includes a comprehensive unit test suite using **xUnit** and **Moq** for testing MediatR handlers in isolation.

### Test Framework Stack
- **xUnit 2.6.4** - Unit testing framework
- **Moq 4.20.70** - Mocking library for dependency isolation
- **Microsoft.EntityFrameworkCore.InMemory** - In-memory database for integration tests

### Test Structure

**Unit Tests** - Located in `tests/LaunchPad.Tests/Handlers/`

#### LoginCommandHandlerTests
- ✅ Valid credentials return JWT token
- ✅ Invalid credentials return empty token
- ✅ AuthService called exactly once (verification)

#### GetAllBooksQueryHandlerTests
- ✅ Returns all books from repository
- ✅ Returns empty list when no books exist
- ✅ Repository.GetAll() called once

#### CreateBookCommandHandlerTests
- ✅ Creates book with valid data
- ✅ Repository.Add() called with correct data
- ✅ Parameterized tests for invalid prices (Theory with InlineData)

### Test Patterns Used

**Arrange-Act-Assert Pattern:**
```csharp
[Fact]
public async Task Handle_WithValidCredentials_ReturnsTokenResponse()
{
    // Arrange - setup mocks and test data
    var user = new User { Id = 1, Username = "testuser" };
    _mockAuthService.Setup(x => x.ValidateUserAsync("testuser", "password123"))
        .ReturnsAsync(user);
    
    var handler = new LoginCommandHandler(_mockAuthService.Object, 
                                          _mockConfiguration.Object, 
                                          _mockLogger.Object);
    var command = new LoginCommand("testuser", "password123");
    
    // Act - execute the handler
    var result = await handler.Handle(command, CancellationToken.None);
    
    // Assert - verify results and mock calls
    Assert.NotEmpty(result.Token);
    _mockAuthService.Verify(x => x.ValidateUserAsync("testuser", "password123"), 
                           Times.Once);
}
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run with verbose output
dotnet test --verbosity normal

# Run specific test class
dotnet test --filter ClassName=LoginCommandHandlerTests
```

### Test Results
- **Total Tests**: 10
- **Status**: ✅ All passing
- **Coverage**: MediatR handlers, query handlers, and command validation

### Future Test Expansion
- Integration tests with EF InMemory DbContext
- E2E tests for API endpoints
- Performance benchmarks
- Data validation tests



## Input Validation

All DTOs and entities include comprehensive data validation using data annotations:

- **Required**: Mandatory fields (Username, Password, Title, Author)
- **StringLength**: Enforces length constraints (3-50 for username, 1-200 for title/author)
- **Range**: Numeric bounds (Price 0.01-10000, Stock 0-100000, Year 1000-2100)
- **RegularExpression**: Pattern matching for ISBN and alphanumeric fields
- **Custom PasswordComplexityAttribute**: Enforces strong passwords
  - Length: 8-128 characters
  - Must include: lowercase, uppercase, digit, special character (@$!%*?&)
  - No spaces or 3+ consecutive identical characters

ModelState validation occurs in controllers before MediatR dispatch.

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
- **GET** `/api/v1.0/books?pageNumber=1&pageSize=10` - Get all books with pagination
  - Query Parameters:
    - `pageNumber` (optional, default: 1) - Page number for pagination
    - `pageSize` (optional, default: 10) - Number of items per page
  - Response: 200 OK with paginated results
  - Response Example:
    ```json
    {
      "items": [
        { "id": 1, "title": "Book 1", "author": "Author 1", "year": 2024 }
      ],
      "pageNumber": 1,
      "pageSize": 10,
      "totalCount": 42,
      "totalPages": 5,
      "hasPreviousPage": false,
      "hasNextPage": true
    }
    ```

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

### Orders (All endpoints require JWT Authorization)
- **POST** `/api/v1.0/orders` - Create a new order (totals computed server-side)
  - Request: `CreateOrderRequestDto` with items
  - Response: 201 Created with OrderDto

- **GET** `/api/v1.0/orders/{id}` - Get order by ID
  - Response: 200 OK with OrderDto or 404 Not Found

- **GET** `/api/v1.0/orders?pageNumber=1&pageSize=10` - Get orders (paginated)
  - Response: 200 OK with list of OrderDto

- **GET** `/api/v1.0/orders/summary` - Aggregated order totals
  - Query Params: `fromDate`, `toDate` (optional)
  - Response: 200 OK with OrderSummaryDto

- **GET** `/api/v1.0/orders/top-items?limit=5` - Top items by revenue
  - Query Params: `limit` (optional, default: 5)
  - Response: 200 OK with list of TopItemDto

## Middleware

- **Exception Handling Middleware** - Centralized error handling with structured error responses (ErrorResponse model)
- Rate limiting and throttling (10 requests per 10 seconds per IP)
- Response compression (gzip)
- **CORS** (Cross-Origin Resource Sharing) - Allows frontend applications from localhost:3000, 5000, 5173, 3001
- JWT Authentication with Bearer tokens
- Swagger/OpenAPI documentation
- Structured logging with Serilog

## Security

### SQL Injection Prevention
✅ **Fully Protected** - All database queries use Entity Framework Core LINQ queries, which generate parameterized SQL automatically:
- No raw SQL queries (`FromSql`, `ExecuteSql`)
- No string concatenation in queries
- No dynamic query building
- All user inputs validated before database operations

### Input Validation
- Data annotations on all DTOs and models (Required, StringLength, Range, RegularExpression)
- Custom `PasswordComplexityAttribute` for strong password enforcement
- Controller-level ModelState validation before MediatR dispatch
- Type-safe query parameters with integer validation

### Authentication & Authorization
- JWT Bearer tokens with 1-hour expiration
- Passwords hashed with SHA256
- Role-based authorization on protected endpoints
- Sensitive data (tokens, passwords) never logged

### Best Practices
✅ Asynchronous endpoints (no blocking operations)  
✅ Principle of least privilege (repository pattern)  
✅ Secure defaults (validation enforced, logging audited)  
✅ No hardcoded secrets (configuration-based)  

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

### Order
```csharp
public class Order
{
  public int Id { get; set; }
  public DateTime CreatedAt { get; set; }
  public decimal Subtotal { get; set; }
  public decimal DiscountPercent { get; set; }
  public decimal DiscountAmount { get; set; }
  public decimal TaxRate { get; set; }
  public decimal TaxAmount { get; set; }
  public decimal Total { get; set; }
  public ICollection<OrderItem> Items { get; set; }
}
```

### OrderItem
```csharp
public class OrderItem
{
  public int Id { get; set; }
  public int OrderId { get; set; }
  public string ProductName { get; set; }
  public int Quantity { get; set; }
  public decimal UnitPrice { get; set; }
  public decimal LineTotal { get; set; }
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

### Order DTOs
```csharp
public class CreateOrderRequestDto
{
  public decimal DiscountPercent { get; set; }
  public List<CreateOrderItemRequestDto> Items { get; set; }
}

public class CreateOrderItemRequestDto
{
  public string ProductName { get; set; }
  public int Quantity { get; set; }
  public decimal UnitPrice { get; set; }
}

public class OrderDto
{
  public int Id { get; set; }
  public DateTime CreatedAt { get; set; }
  public decimal Subtotal { get; set; }
  public decimal DiscountPercent { get; set; }
  public decimal DiscountAmount { get; set; }
  public decimal TaxRate { get; set; }
  public decimal TaxAmount { get; set; }
  public decimal Total { get; set; }
  public List<OrderItemDto> Items { get; set; }
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

### Prerequisites
- .NET 8.0 SDK or later
- SQL Server (local or remote)
- Visual Studio Code or Visual Studio 2022+ (optional)

### Build the Solution

```bash
# Navigate to project root
cd LaunchPad

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Build for release
dotnet build --configuration Release
```

### Run the Application

```bash
# Run from project root (builds and runs main API)
dotnet run --project src/LaunchPad

# Run tests
dotnet test

# Run tests with verbose output
dotnet test --verbosity normal

# Run specific test class
dotnet test --filter ClassName=LoginCommandHandlerTests
```

### Database Setup

```bash
# Apply migrations to create database schema
cd src/LaunchPad
dotnet ef database update

# Create a migration for new changes
dotnet ef migrations add MigrationName

# Remove last migration
dotnet ef migrations remove
```

### API Access

- **API URL**: `https://localhost:5001`
- **Swagger UI**: `https://localhost:5001/swagger`
- **Health Check**: `https://localhost:5001/health`

### Example Requests

Register a new user:
```bash
curl -X POST https://localhost:5001/api/register \
  -H "Content-Type: application/json" \
  -d '{"username":"john_doe","password":"SecurePass123!@"}'
```

Login and get JWT token:
```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"john_doe","password":"SecurePass123!@"}'
```

Get all books (requires Authorization header):
```bash
curl -X GET https://localhost:5001/api/v1.0/books \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## Getting Started (Original)
1. Clone the repository
2. Configure SQL credentials and Redis connection string in `appsettings.json`
3. Run migrations: `dotnet ef database update`
4. Start the API: `dotnet run`

## Local Development

### Running the API
```bash
cd src/LaunchPad
dotnet run
# API: https://localhost:5001
# Swagger: https://localhost:5001/swagger
```

### Running the Frontend
```bash
cd client
npm install
npm run dev
# React: http://localhost:5173
```

### Database Setup
1. Configure connection string in `appsettings.json`
2. Run migrations: `dotnet ef database update`
3. Database will be created automatically

## Azure Deployment

### Infrastructure as Code (Bicep)

This project uses Azure Bicep for infrastructure deployment:

- `infra/main.bicep`: Entry point for deploying all Azure resources
- `infra/appService.bicep`: Deploys Azure App Service Plan and Web App
- `infra/sqlDatabase.bicep`: Deploys Azure SQL Server and Database
- `infra/resourceGroup.bicep`: Creates Azure resource groups

### Deployment Steps

1. **Create Resource Group:**
```bash
az group create --name launchpad-rg --location canadacentral
```

2. **Deploy Infrastructure:**
```bash
az deployment group create \
  --resource-group launchpad-rg \
  --template-file infra/main.bicep \
  --parameters location=canadacentral \
               sqlLocation=eastus \
               sqlAdminUser=sqladmin \
               sqlAdminPassword='YourSecurePassword123!'
```

3. **Deploy Application:**
```bash
# Publish backend
dotnet publish -c Release -o ./publish

# Deploy to App Service
az webapp deploy \
  --resource-group launchpad-rg \
  --name launchpad-appsvc \
  --src-path ./publish
```

### Bicep Parameters
- `location`: Azure region for App Service (e.g., canadacentral)
- `sqlLocation`: Azure region for SQL Server (e.g., eastus)
- `appServiceName`: Name for App Service (default: launchpad-appsvc)
- `sqlServerName`: Name for SQL Server (default: launchpad-sqlsrv)
- `sqlDbName`: Database name (default: launchpaddb)
- `sqlAdminUser`: SQL admin username
- `sqlAdminPassword`: SQL admin password (secure parameter)
- `appServiceSku`: App Service SKU (default: F1 free tier)

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
- **CQRS Pattern**: MediatR 12.2.0
- **Testing**: xUnit 2.6.4, Moq 4.20.70, EF InMemory
- **Infrastructure**: Azure Bicep for IaC
- **Project Structure**: Multi-project solution (src/ and tests/ directories)

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
