using System;
using System.Collections.Generic;

namespace MyPhamSoul.Models;

public partial class KhachHang
{
    public int MaKhachHang { get; set; }

    public string? TenKhachHang { get; set; }

    public string? DiaChi { get; set; }

    public string? SoDienThoai { get; set; }

    public DateTime? NgaySinh { get; set; }

    public int AccountId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
}
