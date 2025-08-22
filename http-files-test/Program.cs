using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Redirect HTTP requests to HTTPS when possible
//app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// In-memory store for CRUD demo at /forecasts
var store = new ConcurrentDictionary<int, ForecastItem>();
var nextId = 0;
int NewId() => Interlocked.Increment(ref nextId);

// GET all
app.MapGet("/forecasts", () =>
{
    var items = store.Values.OrderBy(f => f.Id);
    return Results.Ok(items);
})
.WithName("GetForecasts");

// GET by id
app.MapGet("/forecasts/{id:int}", (int id) =>
    store.TryGetValue(id, out var item) ? Results.Ok(item) : Results.NotFound())
.WithName("GetForecastById");

// POST create
app.MapPost("/forecasts", (ForecastCreate dto) =>
{
    var id = NewId();
    var item = new ForecastItem(id, dto.Date, dto.TemperatureC, dto.Summary);
    store[id] = item;
    return Results.Created($"/forecasts/{id}", item);
})
.WithName("CreateForecast");

// PUT replace
app.MapPut("/forecasts/{id:int}", (int id, ForecastReplace dto) =>
{
    if (!store.ContainsKey(id)) return Results.NotFound();
    var item = new ForecastItem(id, dto.Date, dto.TemperatureC, dto.Summary);
    store[id] = item;
    return Results.Ok(item);
})
.WithName("ReplaceForecast");

// PATCH partial update
app.MapPatch("/forecasts/{id:int}", (int id, ForecastPatch dto) =>
{
    if (!store.TryGetValue(id, out var existing)) return Results.NotFound();

    var updated = existing with
    {
        Date = dto.Date ?? existing.Date,
        TemperatureC = dto.TemperatureC ?? existing.TemperatureC,
        Summary = dto.Summary ?? existing.Summary
    };

    store[id] = updated;
    return Results.Ok(updated);
})
.WithName("PatchForecast");

// DELETE
app.MapDelete("/forecasts/{id:int}", (int id) =>
    store.TryRemove(id, out _) ? Results.NoContent() : Results.NotFound())
.WithName("DeleteForecast");

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// CRUD models (separate from the demo WeatherForecast above)
internal record ForecastItem(int Id, DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

internal record ForecastCreate(DateOnly Date, int TemperatureC, string? Summary);
internal record ForecastReplace(DateOnly Date, int TemperatureC, string? Summary);
internal record ForecastPatch(DateOnly? Date, int? TemperatureC, string? Summary);
