# LaunchPad - Full Stack API

A modern, pragmatic .NET API with clean architecture, comprehensive testing, and Azure deployment ready.

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK or later
- SQL Server (local or remote)
- Node.js 18+ (for frontend)

### Run Backend
```bash
cd src/LaunchPad
dotnet run
# API runs on https://localhost:5001
# Swagger UI: https://localhost:5001/swagger
```

### Run Frontend (Coming soon)
```bash
cd client
npm install
npm run dev
# React runs on http://localhost:5173
```

### Run Tests
```bash
dotnet test
# All 10 tests pass ✅
```

---

## 📁 Project Structure

```
LaunchPad/ (monorepo)
│
├── src/LaunchPad/                 (Backend API - .NET 8.0)
│   ├── Controllers/               - HTTP endpoints
│   ├── Features/                  - CQRS commands & queries
│   ├── Services/                  - Business logic
│   ├── Models/                    - Domain entities
│   ├── Data/                      - Database context
│   └── README.md                  - Detailed documentation
│
├── tests/LaunchPad.Tests/         (Unit tests)
│   ├── Handlers/                  - MediatR handler tests
│   └── 10+ test cases (all passing)
│
└── client/                        (React frontend - TypeScript)
    ├── src/
    │   ├── components/
    │   ├── pages/
    │   └── services/              - API client
    └── package.json
```

---

## ✨ Features

### API
- ✅ **CQRS Pattern** with MediatR
- ✅ **Clean Architecture** - src/ and tests/ separation
- ✅ **100% Type-Safe** - C# & TypeScript
- ✅ **Security** - JWT authentication, password validation
- ✅ **Testing** - xUnit, Moq, 10 passing tests
- ✅ **Exception Handling** - Middleware with structured responses
- ✅ **Async/Await** - All I/O operations
- ✅ **Logging** - Serilog with structured logs
- ✅ **Rate Limiting** - Built-in protection
- ✅ **CORS** - Configured for frontend access

### DevOps
- ✅ **Infrastructure as Code** - Azure Bicep templates
- ✅ **Azure Ready** - App Service, SQL Database
- ✅ **Git History** - Clean commits, easy to follow
- ✅ **CI/CD Ready** - Azure DevOps pipeline setup

---

## 🏗️ Architecture

**Request Flow:**
```
HTTP Request
    ↓
Controller (thin layer)
    ↓
MediatR (routes to handler)
    ↓
Command/Query Handler (business logic)
    ↓
Service → Repository → Database
    ↓
Response
```

**No over-engineering.** Just simple, working code.

---

## 📚 Documentation

- **[Full API Documentation](./src/LaunchPad/README.md)** - Detailed endpoints, models, and patterns
- **[CQRS Architecture](./src/LaunchPad/README.md#architecture-cqrs-with-mediatr)** - How commands and queries work
- **[Testing Guide](./src/LaunchPad/README.md#testing)** - Run tests and add more
- **[Deployment](./src/LaunchPad/README.md#deployment)** - Deploy to Azure

---

## 🧪 Tests

All unit tests use **xUnit** and **Moq**:

```bash
cd .
dotnet test
```

**Test Results:**
- LoginCommandHandler ✅ (3 tests)
- GetAllBooksQueryHandler ✅ (3 tests)
- CreateBookCommandHandler ✅ (4 tests)
- **Total: 10 tests, 100% passing**

---

## 🔐 Security

- JWT Bearer tokens (1-hour expiration)
- Password complexity validation
- SQL injection prevention (parameterized queries)
- CORS properly configured
- HTTPS enforced (production)
- Input validation on all endpoints

---

## 📊 API Endpoints

### Authentication
```
POST /api/register          - Create new user
POST /api/auth/login        - Get JWT token
```

### Books (requires JWT)
```
GET    /api/v1.0/books      - List all books
GET    /api/v1.0/books/{id} - Get book by ID
POST   /api/v1.0/books      - Create book
PUT    /api/v1.0/books/{id} - Update book
DELETE /api/v1.0/books/{id} - Delete book
```

---

## 🛠️ Tech Stack

### Backend
- **Framework:** .NET 8.0 / ASP.NET Core
- **Database:** SQL Server + Entity Framework Core
- **CQRS:** MediatR 12.2.0
- **Testing:** xUnit 2.6.4, Moq 4.20.70
- **Logging:** Serilog
- **Authentication:** JWT Bearer

### Frontend
- **Framework:** React 18 + TypeScript
- **Build Tool:** Vite
- **HTTP Client:** Fetch API
- **State:** React Hooks

### DevOps
- **Cloud:** Microsoft Azure
- **IaC:** Azure Bicep
- **CI/CD:** Azure DevOps
- **VCS:** GitHub

---

## 🚀 Deployment

### To Azure
```bash
# Update infra/main.bicep with your parameters
az deployment group create \
  --resource-group MyResourceGroup \
  --template-file infra/main.bicep \
  --parameters location=eastus
```

### Environment Variables
```
AZURE_SQL_CONNECTION_STRING=...
JWT_SECRET=...
```

---

## 📈 What's Next

- [ ] Add React frontend with login/book management
- [ ] Add pagination to book list
- [ ] Add Redis caching
- [ ] Deploy to Azure with CI/CD
- [ ] Add integration tests
- [ ] Performance monitoring

---

## 🎓 Learning & Philosophy

This project demonstrates:
- **Pragmatism over dogma** - Only add patterns when they solve real problems
- **Simplicity first** - Don't over-engineer for hypothetical scale
- **Shipping matters** - A working solution beats perfect architecture
- **Fundamentals** - Strong HTTP, database, and security knowledge
- **Responsibility** - Write code you'd maintain in production

**No unnecessary complexity. Just good, working code.** ✅

---

## 📝 License

MIT

---

## 👤 Author

Built with pragmatism and a focus on shipping real value.

**GitHub:** [LaunchPad Repository](https://github.com/mabbasidesign/LaunchPad)

---

## 🤝 Contributing

Questions or improvements? Open an issue or PR.

---

**Last Updated:** February 16, 2026
