using System;
using System.Collections.Generic;

namespace DemoCursorPagination.Models;

public partial class UserNotes
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Note { get; set; } = null!;

    public DateOnly NoteDate { get; set; }

    public virtual Users User { get; set; } = null!;
}