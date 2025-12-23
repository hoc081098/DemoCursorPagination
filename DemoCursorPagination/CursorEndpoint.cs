using DemoCursorPagination.Contracts;
using DemoCursorPagination.Data;
using Microsoft.EntityFrameworkCore;

namespace DemoCursorPagination;

public class CursorEndpoint(
    ApplicationDbContext dbContext,
    ILogger<CursorEndpoint> logger
)
{
    public sealed record Request(
        string? Cursor = null,
        int Limit = 30);

    public async Task<IResult> Handle(Request request,
        CancellationToken cancellationToken = default)
    {
        var limit = request.Limit;
        switch (limit)
        {
            case < 1:
                return Results.BadRequest("Limit must be greater than 0");
            case > 100:
                return Results.BadRequest("Limit must be less than or equal to 100");
        }

        var decodedCursor = Cursor.Decode(request.Cursor);
        logger.LogInformation("Cursor pagination: cursor={@DecodedCursor}, limit={Limit}", decodedCursor, limit);

        // Validate cursor version
        if (decodedCursor is not null && decodedCursor.Version != 1)
        {
            return Results.BadRequest($"Unsupported cursor version: {decodedCursor.Version}. Expected version 1.");
        }

        var query = dbContext.UserNotes.AsNoTracking();
        if (decodedCursor is not null)
        {
            // Use the cursor to fetch the next set of items
            // If we're sorting in ASC order, we'd use '>' instead of '<'.
            query = query.Where(x => EF.Functions.LessThanOrEqual(
                    ValueTuple.Create(x.NoteDate, x.Id),
                    ValueTuple.Create(decodedCursor.Date, decodedCursor.LastId)
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
        var nextCursor = hasMore
            ? Cursor.Encode(
                new Cursor(
                    items[^1].NoteDate,
                    items[^1].Id,
                    Version: 1)
            )
            : null;
        if (hasMore)
        {
            items.RemoveAt(items.Count - 1); // Remove the extra item
        }

        return Results.Ok(new
        {
            Items = items,
            Metadata = new
            {
                Limit = limit,
                HasMore = hasMore,
                NextCursor = nextCursor
            }
        });
    }
}