# Activity 1: Writing and Enhancing API Code with Copilot

## Overview
In this activity, GitHub Copilot was used to scaffold and build the initial User Management API project using ASP.NET Core 8. Rather than writing all boilerplate code manually, Copilot accelerated the setup of the project structure, models, and CRUD endpoints.

---

## How Copilot Assisted

* **Generative scaffolding** for the initial ASP.NET Core Web API project setup without boilerplate errors.
* **Generated the `User` model** with all necessary properties (`Id`, `FirstName`, `LastName`, `Email`, `Role`, `IsActive`, `CreatedAt`, `UpdatedAt`) correctly typed and structured.
* **Generated the in-memory store** inside `UserService.cs`, including the seed data and the `_nextId` counter pattern for auto-incrementing IDs.
* **Generated standard CRUD actions** (`GET`, `POST`, `PUT`, `DELETE`) in `UsersController.cs`, saving time on repetitive code patterns.
* **Suggested route attributes** like `[HttpGet("{id:int}")]` and `[ApiController]` with correct placement automatically.

---

## Example: Copilot Generated User Model

When the developer typed `public class User`, Copilot suggested the full model:

```csharp
public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
```

---

## Example: Copilot Generated CRUD Endpoints

When the developer started typing `[HttpGet]`, Copilot autocompleted the full `GetAll` action including the query filter parameters:

```csharp
[HttpGet]
[ProducesResponseType(typeof(IEnumerable<UserResponseDto>), StatusCodes.Status200OK)]
public IActionResult GetAll([FromQuery] string? role, [FromQuery] bool? isActive)
{
    var result = _users.GetAll(role, isActive);
    return Ok(result);
}
```

---

## Outcome
The initial API was scaffolded significantly faster with Copilot. All four CRUD endpoints were functional with correct HTTP status codes and route patterns on the first run with no manual fixes needed to the generated structure.
