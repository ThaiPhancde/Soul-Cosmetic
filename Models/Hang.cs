using System;
using System.Collections.Generic;

namespace MyPhamSoul.Models;

public partial class Hang
{
    public string MaHang { get; set; } = null!;

    public string? TenHang { get; set; }

    public string? XuatXu { get; set; }

    public virtual ICollection<DanhMucSanPham> DanhMucSanPhams { get; set; } = new List<DanhMucSanPham>();
}
