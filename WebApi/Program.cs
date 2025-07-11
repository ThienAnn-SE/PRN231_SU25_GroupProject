using AppCore.Extensions;
using Repositories.Extensions;
using WebApi.Extension;
using WebApi.Middlewares;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Register repositories and unit of work
builder.Services.AddRepositories(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddServices();

// Add services
builder.Services.AddMemoryCache();
builder.Services.Configure<FingerprintOptions>(builder.Configuration.GetSection("Fingerprint"));
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

// Authentication comes before Authorization
app.UseAuthentication();
app.UseAuthorization();

// Your custom middleware
app.UseMiddleware<FingerprintMiddleware>();

// Finally, endpoints
app.MapControllers();
app.Run();
