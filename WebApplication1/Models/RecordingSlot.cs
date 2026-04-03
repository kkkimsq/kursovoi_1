using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class RecordingSlot
{
    public int SlotId { get; set; }

    public int DoctorId { get; set; }

    public int? PatientId { get; set; }

    public int ServiceId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly StartTime { get; set; }

    public virtual Appointment? Appointment { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Patient? Patient { get; set; }

    public virtual ServicesClinic Service { get; set; } = null!;
}
