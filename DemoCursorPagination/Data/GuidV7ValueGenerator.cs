using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace DemoCursorPagination.Data;

/// <summary>
/// Value generator for GUID v7 (time-ordered UUIDs).
/// Uses the new Guid.CreateVersion7() method introduced in .NET 10.
/// </summary>
public class GuidV7ValueGenerator : ValueGenerator<Guid>
{
    public override bool GeneratesTemporaryValues => false;

    public override Guid Next(EntityEntry entry)
    {
        return Guid.CreateVersion7();
    }
}
