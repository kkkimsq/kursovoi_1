using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Specialty
{
    public int SpecializationId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
