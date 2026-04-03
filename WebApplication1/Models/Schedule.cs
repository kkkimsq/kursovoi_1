using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Schedule
{
    public int ScheduleId { get; set; }

    public int DoctorId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;
}
