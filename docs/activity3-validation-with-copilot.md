# Activity 3: Implementing Validation with Copilot

## Overview
In this activity, GitHub Copilot was used to implement input validation across the User Management API. Validation ensures only clean, correctly formatted data is accepted by the endpoints, rejecting invalid requests before they reach business logic.

---

## How Copilot Assisted

* **Suggested Data Annotation attributes** (`[Required]`, `[EmailAddress]`, `[StringLength]`, `[RegularExpression]`) on the DTO properties automatically when the developer started typing `[`.
* **Recommended using DTOs** (Data Transfer Objects) separately from the `User` model, keeping validation logic out of the database entity.
* **Generated the `[RegularExpression]` pattern** for the `Role` field to restrict values to `Admin`, `User`, or `Moderator`.
* **Suggested the `ValidationProblem(ModelState)` return pattern** which produces structured RFC 7807 error responses consumable by API clients.

---

## Example: Copilot Generated Validation on CreateUserDto

When the developer typed `public string Email`, Copilot suggested the full annotated property:

```csharp
[Required(ErrorMessage = "Email is required.")]
[EmailAddress(ErrorMessage = "Invalid email address format.")]
[StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
public string Email { get; set; } = string.Empty;
```

---

## Full DTO Validation Generated with Copilot

```csharp
public class CreateUserDto
{
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
    public string Email { get; set; } = string.Empty;

    [RegularExpression("^(Admin|User|Moderator)$", ErrorMessage = "Role must be Admin, User, or Moderator.")]
    public string Role { get; set; } = "User";
}
```

---

## Validation Rules Summary

| Field | Rule | Error Message |
|-------|------|---------------|
| `FirstName` | Required, 2–50 chars | "First name must be between 2 and 50 characters." |
| `LastName` | Required, 2–50 chars | "Last name must be between 2 and 50 characters." |
| `Email` | Required, valid format, max 100 chars | "Invalid email address format." |
| `Email` | Must be unique across all users | Returns `409 Conflict` |
| `Role` | Must be `Admin`, `User`, or `Moderator` | "Role must be Admin, User, or Moderator." |

---

## Example Validation Error Response

Sending a `POST /api/users` with a missing email and invalid role returns:

```json
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Email": ["Email is required."],
    "Role": ["Role must be Admin, User, or Moderator."]
  }
}
```

---

## Outcome
All user-facing endpoints now reject invalid data with clear, field-level error messages. Copilot accelerated writing the full set of validation annotations and suggested best practices like separating Create and Update DTOs so update requests can have optional fields.
