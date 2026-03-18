using System.Reflection;
using Microsoft.OpenApi.Models;
using UserManagementApi.Middleware;
using UserManagementApi.Services;

var builder = WebApplication.CreateBuilder(args);

// ──────────────────────────────────────────
// Services
// ──────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddSingleton<IUserService, UserService>();

// Swagger 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "User Management API",
        Version     = "v1",
        Description = "A simple CRUD API for managing users, built with ASP.NET Core 8.",
        Contact     = new OpenApiContact { Name = "Your Name", Email = "you@example.com" }
    });

    
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);


    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Type        = SecuritySchemeType.ApiKey,
        In          = ParameterLocation.Header,
        Name        = "X-Api-Key",
        Description = "Enter your API key in the box below."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ──────────────────────────────────────────
// Middleware pipeline
// ──────────────────────────────────────────

// 1. Request logging (outermost – captures everything)
app.UseMiddleware<RequestLoggingMiddleware>();

// 2. Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management API v1");
    c.RoutePrefix = string.Empty;   
});

// 3. API-key authentication
app.UseMiddleware<ApiKeyAuthMiddleware>();

// 4. Routing + controllers
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
