using WizardworksSquares.Api.Endpoints;
using WizardworksSquares.Api.Middleware;
using WizardworksSquares.Api.Repositories;
using WizardworksSquares.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// DI: repository (data access) + service (business logic)
builder.Services.AddScoped<ISquareRepository, SquareRepository>();
builder.Services.AddScoped<ISquareService, SquareService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dev CORS: allow local React dev servers to call the API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Global exception handler middleware - catches unhandled exceptions
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Swagger: only in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Squares API v1");
        c.RoutePrefix = string.Empty; 
    });
}

// Order matters: CORS before endpoints
app.UseCors("AllowReactApp");
app.UseHttpsRedirection();

app.MapSquareEndpoints();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
   .WithName("HealthCheck")
   .WithTags("Health");

app.Run();
