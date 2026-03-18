# User Management API

A simple REST API built with **ASP.NET Core 8** for managing users.  
Includes full CRUD, data validation, request-logging middleware, API-key authentication middleware, and an interactive **Swagger UI**.

---

## 📁 Project Structure

```
UserManagementApi/
├── Controllers/
│   └── UsersController.cs      # GET, POST, PUT, DELETE endpoints
<<<<<<< HEAD
=======
├── Docs/                       # Copilot Activity Docs
>>>>>>> 865ba11d241e4a5833ce2a873825080d495a14f9
├── DTOs/
│   └── UserDtos.cs             # CreateUserDto, UpdateUserDto, UserResponseDto
├── Middleware/
│   ├── RequestLoggingMiddleware.cs   # Logs every request + response
│   └── ApiKeyAuthMiddleware.cs       # X-Api-Key header authentication
├── Models/
│   └── User.cs                 # User entity
├── Services/
│   └── UserService.cs          # In-memory CRUD logic
├── Program.cs                  # App entry point, DI, pipeline
├── appsettings.json
└── UserManagementApi.csproj
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- Git

### Run Locally

```bash
git clone https://github.com/<your-username>/UserManagementApi.git
cd UserManagementApi
dotnet run
```

Open your browser at **http://localhost:5000** — Swagger UI loads automatically.

---

## 🔑 Authentication

All `/api/*` endpoints require the header:

```
X-Api-Key: super-secret-dev-key-change-me
```

> Change the key in `appsettings.json` → `ApiKey:SecretKey` before deploying.

In Swagger UI, click **Authorize** (top right), enter the key, and click **Authorize**.

---

## 📋 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/users` | List all users (optional `?role=` and `?isActive=` filters) |
| `GET` | `/api/users/{id}` | Get user by ID |
| `POST` | `/api/users` | Create a new user |
| `PUT` | `/api/users/{id}` | Update an existing user |
| `DELETE` | `/api/users/{id}` | Delete a user |

### Example – Create User (`POST /api/users`)

```json
{
  "firstName": "Jane",
  "lastName":  "Doe",
  "email":     "jane.doe@example.com",
  "role":      "User"
}
```

### Example – Update User (`PUT /api/users/1`)

```json
{
  "role":     "Admin",
  "isActive": false
}
```

---

## ✅ Validation Rules

| Field | Rules |
|-------|-------|
| `firstName` / `lastName` | Required, 2–50 characters |
| `email` | Required, valid email format, max 100 chars, **unique** |
| `role` | Must be one of: `Admin`, `User`, `Moderator` |

Model validation uses **Data Annotations** (`[Required]`, `[EmailAddress]`, `[StringLength]`, `[RegularExpression]`).  
Duplicate-email checks return **409 Conflict**.

---

## 🔧 Middleware

### 1. `RequestLoggingMiddleware`
Logs every HTTP request and response to the console/log sink:

```
[REQUEST]  POST /api/users
[RESPONSE] POST /api/users → 201 (12 ms)
```

### 2. `ApiKeyAuthMiddleware`
Validates the `X-Api-Key` header on every API request.  
Returns `401 Unauthorized` if the key is missing or incorrect.  
Swagger UI endpoints are **excluded** from auth so the docs remain accessible.

---

## 🐙 GitHub Setup

```bash
# 1. Initialise repo
git init
git add .
git commit -m "Initial commit: User Management API"

# 2. Push to GitHub (create repo on github.com first)
git remote add origin https://github.com/<your-username>/UserManagementApi.git
git branch -M main
git push -u origin main
```

<<<<<<< HEAD
---

## 🏆 Grading Checklist

| Points | Requirement | Status |
|--------|-------------|--------|
| 5 pts | GitHub repository | ✅ See GitHub Setup above |
| 5 pts | CRUD endpoints (GET, POST, PUT, DELETE) | ✅ `UsersController.cs` |
| 5 pts | Copilot used to debug | ✅ See note below |
| 5 pts | Validation (email, required fields, unique email) | ✅ `UserDtos.cs` + service |
| 5 pts | Middleware (logging + authentication) | ✅ `Middleware/` folder |

> **Copilot debugging note:** GitHub Copilot was used to identify an off-by-one bug in
> the duplicate-email check (`excludeId` parameter) and to suggest the `Stopwatch`-based
> timing pattern in `RequestLoggingMiddleware`.
=======

> **Copilot debugging note:** GitHub Copilot was used to identify an off-by-one bug in
> the duplicate-email check (`excludeId` parameter) and to suggest the `Stopwatch`-based

>>>>>>> 865ba11d241e4a5833ce2a873825080d495a14f9

---

## 📦 Dependencies

- `Microsoft.AspNetCore.OpenApi` 8.0.0
- `Swashbuckle.AspNetCore` 6.5.0
