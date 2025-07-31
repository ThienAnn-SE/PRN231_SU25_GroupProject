using ApiService.Extension;
using AppCore.BaseModel;
using AppCore.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories.Extensions;
using System.Text;
using WebApi.Middlewares;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<AuthApiSettings>(
    builder.Configuration.GetSection("AuthApi"));
builder.Services.Configure<RecommendMajorCountOptions>(builder.Configuration.GetSection("RecommendMajorCount"));

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });

    // Add JWT Bearer definition
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token.\n\nExample: Bearer abc123xyz"
    });

    // Require Bearer token globally (optional)
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
// Register repositories and unit of work
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
        var key = Encoding.UTF8.GetBytes(jwtOptions.SecretKey);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddHttpClient(); // If using named client, configure here
builder.Services.AddAuthorization();
// Add services
builder.Services.AddMemoryCache();
builder.Services.AddResponseCompression();
builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MigrateDatabase();

// Add early in the pipeline for cross-cutting concerns
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseResponseCompression();

// Standard middleware
app.UseHttpsRedirection();
app.UseCors();
// Your custom middleware
app.UseMiddleware<AuthMiddleware>();
// Authentication comes before Authorization
app.UseAuthentication();
app.UseAuthorization();

// Finally, endpoints
app.MapControllers();
app.Run();
