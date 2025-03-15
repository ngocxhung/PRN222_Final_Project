using System;
using System.Collections.Generic;

namespace PRN222_Project.Models;

public partial class Department
{
    public int Id { get; set; }

    public string DepartmentName { get; set; } = null!;

    public int? ParentDepartmentId { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
