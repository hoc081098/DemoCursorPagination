using DemoCursorPagination.Contracts;
using DemoCursorPagination.Data;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoCursorPagination;

public sealed class OffsetEndpoint(
    ApplicationDbContext dbContext,
    ILogger<OffsetEndpoint> logger
)
{
    [UsedImplicitly]
    public sealed record Request(
        int Page = 1,
        [FromQuery(Name = "page_size")] int PageSize = 30);

    [UsedImplicitly]
    public sealed record Metadata(
        int Page,
        int PageSize,
        int TotalCount,
        int TotalPages,
        bool HasPreviousPage,
        bool HasNextPage
    );

    [UsedImplicitly]
    public sealed record Response(
        IReadOnlyList<NoteResponse> Items,
        Metadata Metadata);

    public async Task<Results<Ok<Response>, BadRequest<string>>> Handle(Request request,
        CancellationToken cancellationToken = default)
    {
        var page = request.Page;
        var pageSize = request.PageSize;

        logger.LogInformation("Offset pagination: page={Page}, pageSize={PageSize}", page, pageSize);

        if (page < 1)
        {
            return TypedResults.BadRequest("Page must be greater than 0");
        }

        switch (pageSize)
        {
            case < 1:
                return TypedResults.BadRequest("Page size must be greater than 0");
            case > 100:
                return TypedResults.BadRequest("Page size must be less than or equal to 100");
        }

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

        var response = new Response(
            Items: items.Select(x => x.ToResponse()).ToArray(),
            Metadata: new Metadata(
                Page: page,
                PageSize: pageSize,
                TotalCount: totalCount,
                TotalPages: totalPages,
                HasPreviousPage: page > 1,
                HasNextPage: page < totalPages
            )
        );
        return TypedResults.Ok(response);
    }
}
