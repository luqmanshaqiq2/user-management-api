# Activity 4: Implementing Middleware with Copilot

## Overview
In this activity, GitHub Copilot was used to implement two custom middleware components for the User Management API — a **Request Logging Middleware** and an **API Key Authentication Middleware**. Middleware intercepts every HTTP request and response, making it ideal for cross-cutting concerns like logging and security.

---

## How Copilot Assisted

* **Generated the middleware class structure** (`InvokeAsync`, constructor injection of `RequestDelegate` and `ILogger`) when the developer typed `public class RequestLoggingMiddleware`.
* **Suggested `Stopwatch`** from `System.Diagnostics` to measure elapsed time per request automatically.
* **Recommended adaptive log levels** — `LogLevel.Warning` for 4xx responses and `LogLevel.Error` for 5xx — without being asked.
* **Generated the API key header check** pattern using `context.Request.Headers.TryGetValue` for the auth middleware.
* **Suggested the Swagger path bypass** inside `ApiKeyAuthMiddleware` so the Swagger UI remains accessible without an API key.
* **Recommended correct middleware ordering** in `Program.cs` — logging first (outermost), then Swagger, then auth, then routing.

---

## Middleware 1: Request Logging

Copilot generated the full logging middleware when the developer described the requirement in a comment:

```csharp
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();

        _logger.LogInformation("[REQUEST]  {Method} {Path}{Query}",
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString);

        await _next(context);
        sw.Stop();

        var level = context.Response.StatusCode >= 500 ? LogLevel.Error
                  : context.Response.StatusCode >= 400 ? LogLevel.Warning
                  : LogLevel.Information;

        _logger.Log(level, "[RESPONSE] {Method} {Path} → {StatusCode} ({Elapsed} ms)",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            sw.ElapsedMilliseconds);
    }
}
```

### Sample Console Output
```
[REQUEST]  POST /api/users
[RESPONSE] POST /api/users → 201 (14 ms)

[REQUEST]  GET /api/users/99
[RESPONSE] GET /api/users/99 → 404 (3 ms)
```

---

## Middleware 2: API Key Authentication

Copilot generated the header validation and the Swagger bypass:

```csharp
public async Task InvokeAsync(HttpContext context)
{
    // Bypass auth for Swagger UI
    var path = context.Request.Path.Value ?? "";
    if (path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
    {
        await _next(context);
        return;
    }

    if (!context.Request.Headers.TryGetValue("X-Api-Key", out var suppliedKey))
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("{\"error\":\"Missing X-Api-Key header.\"}");
        return;
    }

    var expectedKey = _config["ApiKey:SecretKey"];
    if (!string.Equals(suppliedKey, expectedKey, StringComparison.Ordinal))
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("{\"error\":\"Invalid API key.\"}");
        return;
    }

    await _next(context);
}
```

---

## Middleware Pipeline Order (Copilot Recommended)

```csharp
app.UseMiddleware<RequestLoggingMiddleware>();   // 1. Log everything
app.UseSwagger();                                // 2. Swagger spec
app.UseSwaggerUI(...);                           // 3. Swagger UI
app.UseMiddleware<ApiKeyAuthMiddleware>();        // 4. Authenticate API calls
app.UseRouting();                                // 5. Route to controllers
app.MapControllers();
```

> Copilot warned that placing auth middleware **before** Swagger would block the UI with 401, which was the exact bug encountered in Activity 2.

---

## Outcome
Both middleware components were implemented correctly on the first attempt thanks to Copilot's suggestions. The logging middleware provides full request visibility and the API key middleware secures all endpoints while keeping the developer-facing Swagger UI open.
