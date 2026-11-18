// Built by Pratik Sheth — sample portfolio project.
// .NET 8 Web API demonstrating Clean Architecture: Domain, Application,
// Infrastructure and Api layers over a small product/order catalog.

using System.Text;
using CleanArchitecture.Api.Middleware;
using CleanArchitecture.Application;
using CleanArchitecture.Infrastructure;
using CleanArchitecture.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ----- Layer registrations -------------------------------------------------
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ----- Authentication (JWT bearer) -----------------------------------------
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key is not configured.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

// ----- Swagger / OpenAPI (with Bearer auth support) ------------------------
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Clean Architecture Catalog API",
        Version = "v1",
        Description = "Sample API built with Clean Architecture (.NET 8). " +
                      "Use POST /api/auth/login with the demo credentials from the README to get a token."
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter the JWT returned by /api/auth/login.",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    options.AddSecurityDefinition("Bearer", securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

// ----- Create the SQLite database (and seed data) on first run -------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// ----- HTTP pipeline ---------------------------------------------------------
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Swagger is enabled in all environments for this sample so the API can be
// explored immediately; a production deployment would normally restrict it.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Clean Architecture Catalog API v1");
    options.RoutePrefix = string.Empty; // Serve the UI at the app root.
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
