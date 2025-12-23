using System;
using System.Collections.Generic;

namespace DemoCursorPagination.Models;

public partial class Users
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<UserNotes> UserNotes { get; set; } = new List<UserNotes>();
}