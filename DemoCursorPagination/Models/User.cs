using System;
using System.Collections.Generic;

namespace DemoCursorPagination.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<UserNote> UserNotes { get; set; } = new List<UserNote>();
}
