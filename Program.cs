using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var StoragePath = Path.Combine(Environment.CurrentDirectory, "Storage");
Directory.CreateDirectory(StoragePath);

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(StoragePath),
    RequestPath = "/Storage"
});

app.MapPost("/upload", async (IFormFile file) =>
{
    if(file == null)    return Results.BadRequest("Please provide file");

    using var stream = File.Create(Path.Combine(StoragePath, file.FileName));
    await file.CopyToAsync(stream);

    return Results.Ok("File downloaded successfully");
}).DisableAntiforgery();

app.MapGet("/list", () => Directory.GetFiles(StoragePath).Select(Path.GetFileName));

app.Run();