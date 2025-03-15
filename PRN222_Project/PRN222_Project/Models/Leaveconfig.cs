using System;
using System.Collections.Generic;

namespace PRN222_Project.Models;

public partial class Leaveconfig
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int Level { get; set; }

    public int MaxLeaveDays { get; set; }

    public int UsedLeaveDays { get; set; }

    public int LeaveYear { get; set; }

    public virtual User User { get; set; } = null!;
}
