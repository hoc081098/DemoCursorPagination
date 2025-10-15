using System.Text.Json;
using DemoCursorPagination.Data;

using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
  var connectionString = builder.Configuration.GetConnectionString("Database") ??
   throw new InvalidOperationException("Connection string 'Database' not found.");
  options.UseNpgsql(connectionString);
  options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
});

builder.Services.Configure<JsonOptions>(options =>
{
  // .NET 8 comes with new naming policies for snake_case and kebab-case.
  // Read more at https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8#naming-policies
  options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
  config.DocumentName = "DemoCursorPaginationAPI";
  config.Title = "DemoCursorPaginationAPI v1";
  config.Version = "v1";
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
  app.UseOpenApi();
  app.UseSwaggerUi(config =>
  {
    config.DocumentTitle = "DemoCursorPaginationAPI";
    config.Path = "/swagger";
    config.DocumentPath = "/swagger/{documentName}/swagger.json";
    config.DocExpansion = "list";
  });
}

app.MapGet("/", () => "Hello World!");

app.MapGet("/offset", async (
  ApplicationDbContext dbContext,
  int page = 1,
  int pageSize = 30,
  CancellationToken cancellationToken = default) =>
  {
    if (page < 1) return Results.BadRequest("Page must be greater than 0");
    if (pageSize < 1) return Results.BadRequest("Page size must be greater than 0");
    if (pageSize > 100) return Results.BadRequest("Page size must be less than or equal to 100");

    var query = dbContext.UserNotes
      .OrderByDescending(x => x.NoteDate)
      .ThenByDescending(x => x.Id);

    // Offset pagination typically counts the total number of items
    var totalCount = await query.CountAsync(cancellationToken);
    var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

    // Skip and take the required number of items
    var items = await query
      .Skip((page - 1) * pageSize)
      .Take(pageSize)
      .ToListAsync(cancellationToken);

    return Results.Ok(new
    {
      Items = items,
      // Metadata
      Page = page,
      PageSize = pageSize,
      TotalCount = totalCount,
      TotalPages = totalPages,
      HasPreviousPage = page > 1,
      HasNextPage = page < totalPages
    });
  });

app.Run();

