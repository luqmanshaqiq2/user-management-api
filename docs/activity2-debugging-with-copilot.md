# Activity 2: Debugging Code with Copilot

## Overview
In this activity, GitHub Copilot was used to identify and fix bugs found during development and manual testing of the User Management API. Copilot's inline suggestions and chat explanations helped resolve issues faster than manual debugging alone.

---

## How Copilot Assisted

* **Identified a logic bug** in the `EmailExists()` method where updating a user with their own existing email incorrectly triggered a 409 Conflict response.
* **Suggested the `excludeId` parameter pattern** to fix the duplicate email check so the current user's own record is excluded from the uniqueness query.
* **Caught a `NullReferenceException` risk** in `ApiKeyAuthMiddleware` when the `ApiKey:SecretKey` config value was missing, and suggested a null-guard with a clear 500 error message.
* **Flagged incorrect `ModelState` handling** where `BadRequest()` was used instead of `ValidationProblem(ModelState)`, which causes clients to receive no field-level error details.

---

## Bug #1: Duplicate Email False Positive on Update

### Problem
When a `PUT /api/users/{id}` request was sent with the **same email** the user already had, the API returned `409 Conflict` instead of updating successfully.

### Copilot Diagnosis
Copilot highlighted that `EmailExists()` checked **all** users including the one being updated, so the user's own email always triggered the conflict.

### Before (Buggy Code)
```csharp
public bool EmailExists(string email)
    => _users.Any(u => u.Email == email.ToLowerInvariant());
```

### After (Fixed with Copilot)
```csharp
public bool EmailExists(string email, int? excludeId = null)
    => _users.Any(u => u.Email.Equals(email.Trim().ToLowerInvariant(), StringComparison.Ordinal)
                    && (excludeId is null || u.Id != excludeId));
```

---

## Bug #2: Missing Null Guard on API Key Config

### Problem
If `ApiKey:SecretKey` was missing from `appsettings.json`, the middleware threw an unhandled `NullReferenceException` crashing the request.

### Before (Buggy Code)
```csharp
var expectedKey = _config["ApiKey:SecretKey"];
if (!string.Equals(suppliedKey, expectedKey, StringComparison.Ordinal)) { ... }
```

### After (Fixed with Copilot)
```csharp
var expectedKey = _config["ApiKey:SecretKey"];
if (string.IsNullOrEmpty(expectedKey))
{
    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    await context.Response.WriteAsync("{\"error\":\"API key not configured on server.\"}");
    return;
}
```

---

## Bug #3: Validation Errors Not Returned to Client

### Problem
Invalid POST request bodies (e.g. missing `email`) returned a plain `400 Bad Request` with no details about which fields failed.

### Before
```csharp
if (!ModelState.IsValid)
    return BadRequest();
```

### After (Copilot suggestion)
```csharp
if (!ModelState.IsValid)
    return ValidationProblem(ModelState);
```

This returns a structured RFC 7807 `ProblemDetails` response with per-field error messages.

---

## Outcome
Three bugs were identified and fixed using Copilot during this activity. The fixes improved API correctness, stability under misconfiguration, and developer-friendly error responses.
