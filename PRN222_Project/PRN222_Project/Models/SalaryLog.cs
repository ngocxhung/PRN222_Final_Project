using System;
using System.Collections.Generic;

namespace PRN222_Project.Models;

public partial class SalaryLog
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public int ValidDays { get; set; }

    public int InvalidDays { get; set; }

    public int Month { get; set; }

    public int Year { get; set; }

    public decimal TotalSalary { get; set; }

    public int? ProcessedBy { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public decimal? Bonus { get; set; }

    public string? SalaryLevel { get; set; }

    public int TotalDays { get; set; }

    public virtual User Employee { get; set; } = null!;
}
