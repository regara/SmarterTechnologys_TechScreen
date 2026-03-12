using Microsoft.AspNetCore.Mvc;
using SmarterTechnology.PackageSorter;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/api/sort", ([FromBody] SortRequest request) =>
{
    try
    {
        var stack = PackageSorter
        .Sort(request.Width, request.Height, request.Length, request.Mass)
        .ToString().ToUpperInvariant();

        return Results.Ok(new SortResponse(stack));
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new SortError(ex.Message, ex.ParamName));
    }
});

app.Run();

internal sealed record SortRequest(double Width, double Height, double Length, double Mass);
internal sealed record SortResponse(string Stack);
internal sealed record SortError(string Message, string? ParameterName);
