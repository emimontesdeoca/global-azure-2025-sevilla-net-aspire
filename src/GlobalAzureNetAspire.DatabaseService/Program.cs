using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

// Add PostgreSQL connection string configuration
builder.AddNpgsqlDataSource(connectionName: "postgresdb"); // Add PostgreSQL Aspire integration

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

// Add PostgreSQL connection check endpoint
app.MapGet("/", async (NpgsqlDataSource db) =>
{
    try
    {
        await using var connection = await db.OpenConnectionAsync();
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT NOW()"; // Simple test query
        var result = await cmd.ExecuteScalarAsync();
        return Results.Ok(new { status = "OK", database = "PostgreSQL", result });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            title: "Database connection failed",
            detail: ex.Message,
            statusCode: StatusCodes.Status500InternalServerError);
    }
});

app.Run();