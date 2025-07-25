using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
     .SetBasePath(Directory.GetCurrentDirectory())
     .AddOcelot(builder.Environment);
builder.Services.AddOcelot(builder.Configuration);

// Optional: Swagger for gateway (for inspection only)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<FingerprintOptions>(builder.Configuration.GetSection("Fingerprint"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<FingerprintMiddleware>();
// Must be last
await app.UseOcelot();
app.Run();
