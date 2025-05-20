using System;
using System.Collections.Generic;

namespace MyPhamSoul.Models;

public partial class LoHang
{
    public string MaLoHang { get; set; } = null!;

    public DateTime? NgayNhan { get; set; }

    public string? MaNhaPp { get; set; }

    public double? GiaLo { get; set; }

    public DateTime? HSD { get; set; }

    public int? TrangThaiLH { get; set; }

    public virtual ICollection<ChiTietLoHang> ChiTietLoHangs { get; set; } = new List<ChiTietLoHang>();

    public virtual NhaPhanPhoi? MaNhaPpNavigation { get; set; }
}
