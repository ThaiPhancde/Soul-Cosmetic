using System;
using System.Collections.Generic;

namespace MyPhamSoul.Models;

public partial class ChiTietLoHang
{
    public string MaLoHang { get; set; } = null!;

    public string MaSanPham { get; set; } = null!;

    public int? SoLuong { get; set; }

    public int? DaBan { get; set; }

    public double? GiaNhap { get; set; }

    public DateTime? HSDSP { get; set; }

    public virtual LoHang MaLoHangNavigation { get; set; } = null!;

    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
}
