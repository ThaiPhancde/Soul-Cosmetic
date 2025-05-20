using System;
using System.Collections.Generic;

namespace MyPhamSoul.Models;

public partial class DonHang
{
    public string MaDonHang { get; set; } = null!;

    public double? TongTien { get; set; }

    public int? TrangThaiDonHang { get; set; }

    public int? MaKhachHang { get; set; }

    public bool? ThanhToan { get; set; }

    public int? VanChuyen { get; set; }

    public double? PhiVanChuyen { get; set; }

	public double? TienGiam { get; set; }
    public double? TongThanhToan { get; set; }

    public DateTime? NgayDatHang { get; set; }

    public string? PaymentId { get; set; }

    public string? PaymentMethod { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual KhachHang? MaKhachHangNavigation { get; set; }
}
