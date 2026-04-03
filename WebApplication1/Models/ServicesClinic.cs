using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class ServicesClinic
{
    public int ServiceId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public virtual ICollection<RecordingSlot> RecordingSlots { get; set; } = new List<RecordingSlot>();
}
