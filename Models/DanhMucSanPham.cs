using System;
using System.Collections.Generic;

namespace MyPhamSoul.Models;

public partial class DanhMucSanPham
{
    public string MaDanhMuc { get; set; } = null!;

    public string? TenDanhMuc { get; set; }

    public string? MoTa { get; set; }

    public string? MaCtloai { get; set; }

    public string? MaHang { get; set; }

    public string? HinhAnh { get; set; }

    public double? DanhGia { get; set; }

    public double? Gia { get; set; }

    public string? CachDung { get; set; }

    public string? ChiTiet { get; set; }

    public virtual Ctloai? MaCtloaiNavigation { get; set; }

    public virtual Hang? MaHangNavigation { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
    
}
