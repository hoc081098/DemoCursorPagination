using DemoCursorPagination.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
  var connectionString = builder.Configuration.GetConnectionString("Database") ??
   throw new InvalidOperationException("Connection string 'Database' not found.");
  options.UseNpgsql(connectionString);
  options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
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
app.MapGet("/noteitems", async (ApplicationDbContext db) =>
{
  var items = await db.UserNotes
    .AsNoTracking()
    .Select(x => new {
        x.Id,
        x.Note,
        x.NoteDate,
        x.UserId,
        UserName = x.User.Name
    })
    .ToListAsync();
  return items;
});

app.Run();

