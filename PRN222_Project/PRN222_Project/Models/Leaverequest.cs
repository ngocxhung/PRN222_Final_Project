using System;
using System.Collections.Generic;

namespace PRN222_Project.Models;

public partial class Leaverequest
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateOnly LeaveDateStart { get; set; }

    public DateOnly LeaveDateEnd { get; set; }

    public string Reason { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public virtual User? ApprovedByNavigation { get; set; }

    public virtual User User { get; set; } = null!;
}
