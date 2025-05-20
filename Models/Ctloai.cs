using System;
using System.Collections.Generic;

namespace MyPhamSoul.Models;

public partial class Ctloai
{
    public string MaCtloai { get; set; } = null!;

    public string? TenCtloai { get; set; }

    public string? MaLoai { get; set; }

    public virtual ICollection<DanhMucSanPham> DanhMucSanPhams { get; set; } = new List<DanhMucSanPham>();

    public virtual Loai? MaLoaiNavigation { get; set; }
}
