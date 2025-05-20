using System;

namespace MyPhamSoul.Models
{
    /// <summary>
    /// Đơn vận chuyển tương ứng với mỗi đơn hàng (DonHang)
    /// </summary>
    public class Shipment
    {
        public int Id { get; set; }

        /// <summary>
        /// Mã đơn hàng trong hệ thống (FK DonHang.MaDonHang)
        /// </summary>
        public string OrderId { get; set; } = null!;

        /// <summary>
        /// FK ShippingProvider.Id
        /// </summary>
        public int ProviderId { get; set; }

        /// <summary>
        /// Mã đơn do đối tác cung cấp sau khi tạo đơn vận.
        /// </summary>
        public string? ProviderOrderCode { get; set; }

        public decimal ShippingFee { get; set; }

        /// <summary>
        /// Trạng thái đơn vận (pending, confirmed, delivering, delivered, canceled...)
        /// </summary>
        public string? Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ShippingProvider Provider { get; set; } = null!;

        public virtual DonHang Order { get; set; } = null!;
    }
} 