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

var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.Run();

