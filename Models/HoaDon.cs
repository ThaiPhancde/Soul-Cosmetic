using System;
using System.Collections.Generic;

namespace MyPhamSoul.Models;

public partial class HoaDon
{
    public string MaHoaDon { get; set; } = null!;

    public DateTime? NgayLap { get; set; }

    public double? TongTien { get; set; }

    public string? MaDonHang { get; set; }

    public virtual DonHang? MaDonHangNavigation { get; set; }
}
