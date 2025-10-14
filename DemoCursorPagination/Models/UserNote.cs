using System;
using System.Collections.Generic;

namespace DemoCursorPagination.Models;

public partial class UserNote
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Note { get; set; } = null!;

    public DateOnly NoteDate { get; set; }

    public virtual User User { get; set; } = null!;
}
