using Microsoft.EntityFrameworkCore;

namespace MyPhamSoul.Models
{
    public partial class _2025MyPhamContext : DbContext
    {
        public virtual DbSet<ShippingProvider> ShippingProviders { get; set; } = null!;
        public virtual DbSet<Shipment> Shipments { get; set; } = null!;

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            // ShippingProvider configuration
            modelBuilder.Entity<ShippingProvider>(entity =>
            {
                entity.ToTable("ShippingProviders");

                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.ApiBaseUrl).HasMaxLength(200);
                entity.Property(e => e.ApiKey).HasMaxLength(200);
                entity.Property(e => e.ApiSecret).HasMaxLength(200);
            });

            // Shipment configuration
            modelBuilder.Entity<Shipment>(entity =>
            {
                entity.ToTable("Shipments");

                entity.Property(e => e.OrderId)
                      .HasMaxLength(50)
                      .IsUnicode(false);

                entity.Property(e => e.ProviderOrderCode).HasMaxLength(100);
                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.ShippingFee).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Order)
                      .WithMany(p => p.Shipments)
                      .HasForeignKey(d => d.OrderId)
                      .HasConstraintName("FK_Shipments_DonHang");

                entity.HasOne(d => d.Provider)
                      .WithMany(p => p.Shipments)
                      .HasForeignKey(d => d.ProviderId)
                      .HasConstraintName("FK_Shipments_ShippingProviders");
            });
        }
    }
} 