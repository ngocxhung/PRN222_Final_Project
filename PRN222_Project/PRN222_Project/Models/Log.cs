using System;
using System.Collections.Generic;

namespace PRN222_Project.Models;

public partial class Log
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Action { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime Timestamp { get; set; }

    public virtual User User { get; set; } = null!;
}
