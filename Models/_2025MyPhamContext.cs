using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MyPhamSoul.Models;

public partial class _2025MyPhamContext : DbContext
{
    public _2025MyPhamContext()
    {
    }

    public _2025MyPhamContext(DbContextOptions<_2025MyPhamContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<ChiTietLoHang> ChiTietLoHangs { get; set; }

    public virtual DbSet<Ctloai> Ctloais { get; set; }

    public virtual DbSet<DanhMucSanPham> DanhMucSanPhams { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<Hang> Hangs { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<LoHang> LoHangs { get; set; }

    public virtual DbSet<Loai> Loais { get; set; }

    public virtual DbSet<NhaPhanPhoi> NhaPhanPhois { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }
	public virtual DbSet<DanhGia> DanhGias { get; set; }

    public virtual DbSet<KhuyenMai> KhuyenMais { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)


             => optionsBuilder.UseSqlServer("Server=LEVIAS;Database=Soul;Integrated Security=True; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Account");

            entity.Property(e => e.AccountEmail).HasMaxLength(50);
            entity.Property(e => e.AccountPassword).HasMaxLength(50);
            entity.Property(e => e.AnhDaiDien).HasMaxLength(250);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.DiaChi).HasMaxLength(500);
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.LastLogin).HasColumnType("datetime");
            entity.Property(e => e.NgaySinh).HasColumnType("datetime");
            entity.Property(e => e.Phone)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.Salt)
                .HasMaxLength(6)
                .IsFixedLength();

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_Account_Role");
        });

        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => new { e.MaDonHang, e.MaSanPham });

            entity.ToTable("ChiTietDonHang");

            entity.Property(e => e.MaDonHang)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaSanPham)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaDonHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietDonHang_DonHang");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietDonHang_SanPham");
        });

        modelBuilder.Entity<KhuyenMai>(entity =>
        {
            entity.HasKey(e => e.MaKM);

            entity.ToTable("KhuyenMai");

            entity.Property(e => e.MaKM)
                .HasMaxLength(10)
                .IsFixedLength();

            entity.Property(e => e.TenKM)
                .HasMaxLength(50)
                .IsFixedLength();

            entity.Property(e => e.AccountId)
                .HasMaxLength(10)
                .IsFixedLength();

            entity.Property(e => e.GiaGiam)
                .HasMaxLength(10)
                .IsFixedLength();

            entity.Property(e => e.NgayBD).HasColumnType("datetime");
            entity.Property(e => e.NgayKT).HasColumnType("datetime");

        });

        modelBuilder.Entity<ChiTietLoHang>(entity =>
        {
            entity.HasKey(e => new { e.MaLoHang, e.MaSanPham });

            entity.ToTable("ChiTietLoHang");

            entity.Property(e => e.MaLoHang)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaSanPham)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.SoLuong)
                .HasMaxLength(50);

            entity.HasOne(d => d.MaLoHangNavigation).WithMany(p => p.ChiTietLoHangs)
                .HasForeignKey(d => d.MaLoHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietLoHang_LoHang");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.ChiTietLoHangs)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietLoHang_SanPham");
        });

        modelBuilder.Entity<Ctloai>(entity =>
        {
            entity.HasKey(e => e.MaCtloai);

            entity.ToTable("CTLoai");

            entity.Property(e => e.MaCtloai)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("MaCTLoai");
            entity.Property(e => e.MaLoai)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TenCtloai)
                .HasMaxLength(250)
                .HasColumnName("TenCTLoai");

            entity.HasOne(d => d.MaLoaiNavigation).WithMany(p => p.Ctloais)
                .HasForeignKey(d => d.MaLoai)
                .HasConstraintName("FK_CTLoai_Loai");
        });

        modelBuilder.Entity<DanhMucSanPham>(entity =>
        {
            entity.HasKey(e => e.MaDanhMuc);

            entity.ToTable("DanhMucSanPham");

            entity.Property(e => e.MaDanhMuc)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.HinhAnh).HasMaxLength(250);
            entity.Property(e => e.MaCtloai)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("MaCTLoai");
            entity.Property(e => e.MaHang)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TenDanhMuc).HasMaxLength(250);

            entity.HasOne(d => d.MaCtloaiNavigation).WithMany(p => p.DanhMucSanPhams)
                .HasForeignKey(d => d.MaCtloai)
                .HasConstraintName("FK_DanhMucSanPham_CTLoai");

            entity.HasOne(d => d.MaHangNavigation).WithMany(p => p.DanhMucSanPhams)
                .HasForeignKey(d => d.MaHang)
                .HasConstraintName("FK_DanhMucSanPham_Hang");
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.MaDonHang);

            entity.ToTable("DonHang");

            entity.Property(e => e.MaDonHang)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NgayDatHang).HasColumnType("datetime");

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaKhachHang)
                .HasConstraintName("FK_DonHang_KhachHang");
        });

		modelBuilder.Entity<DanhGia>(entity =>
		{
			entity.HasKey(e => e.MaDanhGia);

			entity.ToTable("DanhGia");

			entity.Property(e => e.MaDanhGia)
				.HasMaxLength(10)
				.IsFixedLength();

			entity.Property(e => e.MaDanhGia)

				.HasColumnName("MaDanhGia")
				.IsFixedLength();

			entity.Property(e => e.HinhAnh1)

				.HasColumnName("HinhAnh1")
				.IsFixedLength();

			entity.Property(e => e.HinhAnh2)

				.HasColumnName("HinhAnh2")
				.IsFixedLength();

			entity.Property(e => e.HinhAnh3)

				.HasColumnName("HinhAnh3")
				.IsFixedLength();

			entity.Property(e => e.HinhAnh4)

				.HasColumnName("HinhAnh4")
				.IsFixedLength();

			entity.Property(e => e.HinhAnh5)

				.HasColumnName("HinhAnh5")
				.IsFixedLength();

			entity.Property(e => e.Video)

				.HasColumnName("Video")
				.IsFixedLength();

			entity.Property(e => e.HoTen)

				.HasColumnName("HoTen")
				.IsFixedLength();

			entity.Property(e => e.MaDanhMuc)

				.HasColumnName("MaDanhMuc")
				.IsFixedLength();

			entity.Property(e => e.NoiDung)
				.HasMaxLength(200)
				.HasColumnName("NoiDung")
				.IsFixedLength();

			entity.Property(e => e.Diem)

				.HasColumnName("Diem")
				.IsFixedLength();

			entity.Property(e => e.NgayDG).HasColumnType("datetime");
		});

		modelBuilder.Entity<Hang>(entity =>
        {
            entity.HasKey(e => e.MaHang);

            entity.ToTable("Hang");

            entity.Property(e => e.MaHang)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TenHang).HasMaxLength(250);
            entity.Property(e => e.XuatXu).HasMaxLength(50);
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon);

            entity.ToTable("HoaDon");

            entity.Property(e => e.MaHoaDon)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaDonHang)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NgayLap).HasColumnType("date");

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaDonHang)
                .HasConstraintName("FK_HoaDon_DonHang");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKhachHang);

            entity.ToTable("KhachHang");

            entity.Property(e => e.NgaySinh).HasColumnType("datetime");
            entity.Property(e => e.SoDienThoai).HasMaxLength(50);
            entity.Property(e => e.TenKhachHang).HasMaxLength(250);

            entity.HasOne(d => d.Account).WithMany(p => p.KhachHangs)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KhachHang_Account");
        });

        modelBuilder.Entity<LoHang>(entity =>
        {
            entity.HasKey(e => e.MaLoHang);

            entity.ToTable("LoHang");

            entity.Property(e => e.MaLoHang)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaNhaPp)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("MaNhaPP");
            entity.Property(e => e.NgayNhan).HasColumnType("date");

            entity.HasOne(d => d.MaNhaPpNavigation).WithMany(p => p.LoHangs)
                .HasForeignKey(d => d.MaNhaPp)
                .HasConstraintName("FK_LoHang_NhaPhanPhoi");
        });

        modelBuilder.Entity<Loai>(entity =>
        {
            entity.HasKey(e => e.MaLoai);

            entity.ToTable("Loai");

            entity.Property(e => e.MaLoai)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TenLoai).HasMaxLength(250);
        });

        modelBuilder.Entity<NhaPhanPhoi>(entity =>
        {
            entity.HasKey(e => e.MaNhaPp);

            entity.ToTable("NhaPhanPhoi");

            entity.Property(e => e.MaNhaPp)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("MaNhaPP");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Sdt)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SDT");
            entity.Property(e => e.TenNhaPp)
                .HasMaxLength(250)
                .HasColumnName("TenNhaPP");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK_Role");

            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.MaSanPham);

            entity.ToTable("SanPham");

            entity.Property(e => e.MaSanPham)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaDanhMuc)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Mau).HasMaxLength(50);
            entity.Property(e => e.NgaySua).HasColumnType("datetime");
            entity.Property(e => e.NgaySx)
                .HasColumnType("datetime")
                .HasColumnName("NgaySX");
            entity.Property(e => e.Slkho).HasColumnName("SLKho");
            entity.Property(e => e.TenSanPham).HasMaxLength(250);

            entity.HasOne(d => d.MaDanhMucNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaDanhMuc)
                .HasConstraintName("FK_SanPham_DanhMucSanPham");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
