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

app.MapGet("/readme", () =>
{
    var readmePath = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "README.md"));
    if (!File.Exists(readmePath))
    {
        return Results.NotFound("README.md could not be found.");
    }

    var markdown = File.ReadAllText(readmePath);
    var renderedBody = ReadmeRenderer.ToHtml(markdown);

    var page = $$"""
                 <!doctype html>
                 <html lang="en">
                 <head>
                     <meta charset="utf-8" />
                     <meta name="viewport" content="width=device-width, initial-scale=1" />
                     <title>README | Smarter Technology Package Sorter</title>
                     <link rel="stylesheet" href="/readme.css" />
                 </head>
                 <body>
                     <div class="readme-shell">
                         <header class="readme-header">
                             <p class="eyebrow">Documentation</p>
                             <div class="readme-actions">
                                 <a class="back-link" href="/">Back to Sorter</a>
                                 <a class="repo-link" href="https://github.com/regara/SmarterTechnologys_TechScreen" target="_blank" rel="noopener noreferrer">GitHub Repo</a>
                             </div>
                         </header>
                         <article class="readme-body">
                             {{renderedBody}}
                         </article>
                     </div>
                 </body>
                 </html>
                 """;

    return Results.Content(page, "text/html; charset=utf-8");
});

app.Run();

internal sealed record SortRequest(double Width, double Height, double Length, double Mass);
internal sealed record SortResponse(string Stack);
internal sealed record SortError(string Message, string? ParameterName);
