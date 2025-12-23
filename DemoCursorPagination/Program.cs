using System.Text.Json;
using DemoCursorPagination;
using DemoCursorPagination.Data;
using Microsoft.EntityFrameworkCore;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Database") ??
                           throw new InvalidOperationException("Connection string 'Database' not found.");
    options.UseNpgsql(connectionString);
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
});

builder.Services.AddScoped<CursorEndpoint>();
builder.Services.AddScoped<OffsetEndpoint>();

builder.Services.Configure<JsonOptions>(options =>
{
    // .NET 8+ comes with new naming policies for snake_case and kebab-case.
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

app.MapGet("/offset",
    (
        [AsParameters] OffsetEndpoint.Request request,
        OffsetEndpoint endpoint,
        CancellationToken cancellationToken = default
    ) => endpoint.Handle(request, cancellationToken));

app.MapGet("/cursor",
    (
        [AsParameters] CursorEndpoint.Request request,
        CursorEndpoint endpoint,
        CancellationToken cancellationToken = default
    ) => endpoint.Handle(request, cancellationToken));

app.Run();