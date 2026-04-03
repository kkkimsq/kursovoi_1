using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Doctor
{
    public int DoctorId { get; set; }

    public int SpecializationId { get; set; }

    public string FullName { get; set; } = null!;

    public virtual ICollection<RecordingSlot> RecordingSlots { get; set; } = new List<RecordingSlot>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual Specialty Specialization { get; set; } = null!;
}
