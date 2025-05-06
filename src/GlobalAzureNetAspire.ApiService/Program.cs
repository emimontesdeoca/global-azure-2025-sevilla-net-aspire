using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddMemoryCache(); // Add memory caching service

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];
const string CacheKey = "WeatherForecast"; // Cache key constant

app.MapGet("/weatherforecast", (IMemoryCache cache) =>
{
    if (!cache.TryGetValue(CacheKey, out WeatherForecast[] forecast))
    {
        forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();
        cache.Set(CacheKey, forecast);
    }
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapPost("/cache/clear", (IMemoryCache cache) =>
{
    cache.Remove(CacheKey);
    return Results.NoContent();
})
.WithName("ClearWeatherForecastCache");

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}