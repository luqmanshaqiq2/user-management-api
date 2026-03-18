namespace UserManagementApi.Middleware;

/// <summary>
/// Simple API-key authentication middleware.
/// Reads the expected key from configuration ("ApiKey:SecretKey").
/// Clients must supply the header:  X-Api-Key: &lt;key&gt;
/// </summary>
public class ApiKeyAuthMiddleware
{
    private const string ApiKeyHeader = "X-Api-Key";

    private readonly RequestDelegate _next;
    private readonly IConfiguration  _config;
    private readonly ILogger<ApiKeyAuthMiddleware> _logger;

    public ApiKeyAuthMiddleware(RequestDelegate next, IConfiguration config,
        ILogger<ApiKeyAuthMiddleware> logger)
    {
        _next   = next;
        _config = config;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Allow Swagger UI / OpenAPI endpoints through without auth
        var path = context.Request.Path.Value ?? "";
        if (path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/scalar", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(ApiKeyHeader, out var suppliedKey))
        {
            _logger.LogWarning("Request rejected – missing {Header} header.", ApiKeyHeader);
            context.Response.StatusCode  = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(
                """{"error":"Unauthorized","detail":"Missing X-Api-Key header."}""");
            return;
        }

        var expectedKey = _config["ApiKey:SecretKey"];
        if (!string.Equals(suppliedKey, expectedKey, StringComparison.Ordinal))
        {
            _logger.LogWarning("Request rejected – invalid API key supplied.");
            context.Response.StatusCode  = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(
                """{"error":"Unauthorized","detail":"Invalid API key."}""");
            return;
        }

        await _next(context);
    }
}
