using System;
using System.Collections.Generic;

namespace PRN222_Project.Models;

public partial class Checkout
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string ActionType { get; set; } = null!;

    public DateTime? CheckInTime { get; set; }

    public DateTime? CheckOutTime { get; set; }

    public DateOnly LogDate { get; set; }

    public byte EmailSent { get; set; }

    public string Status { get; set; } = null!;

    public string? Reason { get; set; }

    public virtual User User { get; set; } = null!;
}
