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
  CancellationToken cancellationToken = default
  ) =>
{
    Console.WriteLine($"Offset pagination: page={page}, pageSize={pageSize}");

    if (page < 1) return Results.BadRequest("Page must be greater than 0");
    if (pageSize < 1) return Results.BadRequest("Page size must be greater than 0");
    if (pageSize > 100) return Results.BadRequest("Page size must be less than or equal to 100");

    var query = dbContext.UserNotes
      .AsNoTracking()
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


app.MapGet("/cursor", async (
  ApplicationDbContext dbContext,
  string? cursor = null,
  int limit = 30,
  CancellationToken cancellationToken = default
) =>
{
    if (limit < 1) return Results.BadRequest("Limit must be greater than 0");
    if (limit > 100) return Results.BadRequest("Limit must be less than or equal to 100");

    var decodedCursor = DemoCursorPagination.Cursor.Decode(cursor);
    Console.WriteLine($"Cursor pagination: cursor={decodedCursor}, limit={limit}");

    var query = dbContext.UserNotes.AsNoTracking();
    if (decodedCursor is not null)
    {
        // Use the cursor to fetch the next set of items
        // If we sorting in ASC order, we'd use '>' instead of '<'.
        query = query.Where(
          x => EF.Functions.LessThanOrEqual(
            ValueTuple.Create(x.NoteDate, x.Id),
            ValueTuple.Create(decodedCursor.Date, Guid.Parse(decodedCursor.LastId))
          )
        );
    }

    // Fetch the items and determine if there are more
    var items = await query
      .OrderByDescending(x => x.NoteDate)
      .ThenByDescending(x => x.Id)
      .Take(limit + 1) // Take one extra item to check if there are more
      .ToListAsync(cancellationToken);

    // Extract the cursor and ID for the next page
    var hasMore = items.Count > limit;
    string? nextCursor = hasMore
        ? DemoCursorPagination.Cursor.Encode(
          new DemoCursorPagination.Cursor(
            items[^1].NoteDate,
            items[^1].Id.ToString())
          )
        : null;
    if (hasMore)
    {
        items.RemoveAt(items.Count - 1); // Remove the extra item
    }

    return Results.Ok(new
    {
        Items = items,
        // Metadata
        Limit = limit,
        HasMore = hasMore,
        NextCursor = nextCursor
    });
});

app.Run();
