using System;
using System.Collections.Generic;

namespace PRN222_Project.Models;

public partial class SalaryLevel
{
    public int Id { get; set; }

    public int Level { get; set; }

    public decimal DailySalary { get; set; }

    public decimal MonthlySalary { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
