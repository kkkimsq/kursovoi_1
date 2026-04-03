using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Administrator
{
    public int AdminId { get; set; }

    public string FullName { get; set; } = null!;

    public string Login { get; set; } = null!;
    public byte[] Password { get; set; }
}
