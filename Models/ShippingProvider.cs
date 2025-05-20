using System.Collections.Generic;

namespace MyPhamSoul.Models
{
    /// <summary>
    /// Thông tin nhà vận chuyển (GHN, GHTK, ...)
    /// </summary>
    public class ShippingProvider
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        /// <summary>
        /// Base url của API (sandbox hoặc production)
        /// </summary>
        public string? ApiBaseUrl { get; set; }

        /// <summary>
        /// API Token / Key để xác thực
        /// </summary>
        public string? ApiKey { get; set; }

        /// <summary>
        /// Api secret nếu có (một số đối tác cần secret khi ký chữ ký)
        /// </summary>
        public string? ApiSecret { get; set; }

        /// <summary>
        /// Cờ cho biết đây là môi trường Sandbox (test)
        /// </summary>
        public bool IsSandbox { get; set; }

        public virtual ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    }
} 