using FluentValidation;
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NebuloMongo.Application.Settings;
using NebuloMongo.Application.UseCase;
using NebuloMongo.Application.Validators;
using NebuloMongo.Infrastructure.Context;
using NebuloMongo.Infrastructure.Swagger;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// API VERSIONING
// ==========================================
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// ==========================================
// SWAGGER + VERSIONING
// ==========================================
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
builder.Services.AddEndpointsApiExplorer();

// ==========================================
// JWT
// ==========================================
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var issuer = builder.Configuration["Jwt:Issuer"];
        var audience = builder.Configuration["Jwt:Audience"];
        var key = builder.Configuration["Jwt:Key"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

builder.Services.AddAuthorization();

// ==========================================
// REPOSITORIES
// ==========================================
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// ==========================================
// USE CASES
// ==========================================
builder.Services.AddScoped<UserUseCase>();
builder.Services.AddScoped<AuthUseCase>();

// ==========================================
// CONTROLLERS
// ==========================================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// ==========================================
// VALIDATION
// ==========================================
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RequestUserValidator>();

// ==========================================
// SWAGGER
// ==========================================
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Digite: Bearer {token}",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ==========================================
// MONGO SETTINGS + CONTEXT
// ==========================================
builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoSettings"));

builder.Services.AddSingleton<MongoContext>();

// ==========================================
// HEALTH CHECKS (SEM MONGO)
// ==========================================
builder.Services.AddHealthChecks();

builder.Services.AddHealthChecksUI()
    .AddInMemoryStorage();

// ==========================================
// BUILD
// ==========================================
var app = builder.Build();

// ==========================================
// SWAGGER
// ==========================================
if (app.Environment.IsDevelopment())
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var desc in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{desc.GroupName}/swagger.json",
                $"Nebulo API {desc.GroupName.ToUpper()}"
            );
        }

        options.RoutePrefix = string.Empty;
    });
}

// ==========================================
// PIPELINE
// ==========================================
app.UseAuthentication();
app.UseAuthorization();

// JSON do health
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Dashboard UI
app.MapHealthChecksUI(options =>
{
    options.UIPath = "/hc-ui";
});

app.MapControllers();

app.Run();
