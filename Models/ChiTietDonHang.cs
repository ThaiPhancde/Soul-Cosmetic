using System;
using System.Collections.Generic;

namespace MyPhamSoul.Models;

public partial class ChiTietDonHang
{
    public string MaDonHang { get; set; } = null!;

    public string MaSanPham { get; set; } = null!;

    public int? SoLuong { get; set; }

    public double? GiaBan { get; set; }

    public virtual DonHang MaDonHangNavigation { get; set; } = null!;

    public virtual SanPham MaSanPhamNavigation { get; set; } = null!;
}
