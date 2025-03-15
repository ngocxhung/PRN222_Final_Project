using System;
using System.Collections.Generic;

namespace PRN222_Project.Models;

public partial class EmployeeDataLog
{
    public int Id { get; set; }

    public string FileName { get; set; } = null!;

    public string ActionType { get; set; } = null!;

    public DateTime ActionTime { get; set; }

    public int PerformedBy { get; set; }

    public int TotalRecords { get; set; }

    public virtual User PerformedByNavigation { get; set; } = null!;
}
