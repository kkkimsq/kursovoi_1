using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public string? Diagnosis { get; set; }

    public string? Recommendations { get; set; }

    public decimal? FinalCost { get; set; }

    public DateTime? CreationDate { get; set; }

    public virtual RecordingSlot AppointmentNavigation { get; set; } = null!;
}
