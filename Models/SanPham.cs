using System;
using System.Collections.Generic;

namespace MyPhamSoul.Models;

public partial class SanPham
{
    public string MaSanPham { get; set; } = null!;

    public string? TenSanPham { get; set; }

    public string? Mau { get; set; }

    public string? Anh { get; set; }

    public double? Gia { get; set; }

    

    public int? Slkho { get; set; }

    public void UpdateSlkho()
    {
        // Tính tổng số lượng từ ChiTietLoHangs
        Slkho = ChiTietLoHangs.Sum(lh => lh.SoLuong ?? 0);
    }

    public DateTime? NgaySx { get; set; }

    public int? LuotMua { get; set; }

    public string? MaDanhMuc { get; set; }

    public bool? HomeFlag { get; set; }

    public bool? BestSellers { get; set; }

    public string? MoTa { get; set; }

    

    public DateTime? NgaySua { get; set; }

    public double? GiaNhap { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<ChiTietLoHang> ChiTietLoHangs { get; set; } = new List<ChiTietLoHang>();

    public virtual DanhMucSanPham? MaDanhMucNavigation { get; set; }
}
