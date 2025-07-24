using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
     .SetBasePath(Directory.GetCurrentDirectory())
     .AddOcelot(builder.Environment);
builder.Services.AddOcelot(builder.Configuration);

// Optional: Swagger for gateway (for inspection only)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Must be last
await app.UseOcelot();
app.Run();
