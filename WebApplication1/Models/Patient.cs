using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Patient
{
    public int PatientId { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public string? Phone { get; set; }

    public string? InsuranceNumber { get; set; }

    public string? Gender { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public virtual ICollection<RecordingSlot> RecordingSlots { get; set; } = new List<RecordingSlot>();
}
