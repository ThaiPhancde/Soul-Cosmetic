using System;
using System.Collections.Generic;

namespace MyPhamSoul.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public string? Phone { get; set; }

    public string? AccountEmail { get; set; }

    public string? AccountPassword { get; set; }

    public string? Salt { get; set; }

    public bool Active { get; set; }

    public string? FullName { get; set; }

    public int? RoleId { get; set; }

    public DateTime? LastLogin { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? NgaySinh { get; set; }

    public string? AnhDaiDien { get; set; }

    public string? DiaChi { get; set; }

    public bool? GioiTinh { get; set; }

    public virtual ICollection<KhachHang> KhachHangs { get; set; } = new List<KhachHang>();

    public virtual Role? Role { get; set; }
}
