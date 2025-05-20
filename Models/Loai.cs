using System;
using System.Collections.Generic;

namespace MyPhamSoul.Models;

public partial class Loai
{
    public string MaLoai { get; set; } = null!;

    public string? TenLoai { get; set; }

    public virtual ICollection<Ctloai> Ctloais { get; set; } = new List<Ctloai>();
}
