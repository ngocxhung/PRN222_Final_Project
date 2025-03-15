using System;
using System.Collections.Generic;

namespace PRN222_Project.Models;

public partial class User
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string Password { get; set; } = null!;

    public int? DepartmentId { get; set; }

    public int? RoleId { get; set; }

    public string Status { get; set; } = null!;

    public int? SalaryLevelId { get; set; }

    public string EmploymentType { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public DateOnly? BirthDate { get; set; }

    public string? Avatar { get; set; }

    public int? PositionId { get; set; }

    public virtual ICollection<Checkout> Checkouts { get; set; } = new List<Checkout>();

    public virtual Department? Department { get; set; }

    public virtual ICollection<Emailconfig> Emailconfigs { get; set; } = new List<Emailconfig>();

    public virtual ICollection<EmployeeDataLog> EmployeeDataLogs { get; set; } = new List<EmployeeDataLog>();

    public virtual ICollection<Leaveconfig> Leaveconfigs { get; set; } = new List<Leaveconfig>();

    public virtual ICollection<Leaverequest> LeaverequestApprovedByNavigations { get; set; } = new List<Leaverequest>();

    public virtual ICollection<Leaverequest> LeaverequestUsers { get; set; } = new List<Leaverequest>();

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();

    public virtual Position? Position { get; set; }

    public virtual Role? Role { get; set; }

    public virtual SalaryLevel? SalaryLevel { get; set; }

    public virtual ICollection<SalaryLog> SalaryLogs { get; set; } = new List<SalaryLog>();
}
