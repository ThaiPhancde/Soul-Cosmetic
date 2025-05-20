using System;
using System.Collections.Generic;

namespace MyPhamSoul.Models;

public partial class NhaPhanPhoi
{
    public string MaNhaPp { get; set; } = null!;

    public string? TenNhaPp { get; set; }

    public string? DiaChi { get; set; }

    public string? Sdt { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<LoHang> LoHangs { get; set; } = new List<LoHang>();
}
