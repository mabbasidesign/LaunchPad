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
- Rate limiting and throttling
- Distributed Redis caching for API responses
- Response compression
- Exception handling and ProblemDetails
- Health checks

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

## Contributing
Pull requests and issues are welcome. Please follow DDD and .NET best practices.

## License
MIT
