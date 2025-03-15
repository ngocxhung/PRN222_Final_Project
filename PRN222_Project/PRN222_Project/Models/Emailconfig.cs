using System;
using System.Collections.Generic;

namespace PRN222_Project.Models;

public partial class Emailconfig
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public TimeOnly CheckInTime { get; set; }

    public TimeOnly CheckOutTime { get; set; }

    public byte[] LastEditConfig { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
