using System.Threading.Tasks;

namespace MyPhamSoul.Services
{
    public class ShippingRateRequest
    {
        public int FromDistrictId { get; set; }
        public int ToDistrictId { get; set; }
        public string? ToWardCode { get; set; }
        public int Weight { get; set; } // gram
        public decimal OrderValue { get; set; }
    }

    public class ShipmentRequest
    {
        public string OrderCode { get; set; } = null!; // mã đơn hàng trong hệ thống
        public int FromDistrictId { get; set; }
        public int ToDistrictId { get; set; }
        public string ToWardCode { get; set; } = null!;
        public string ToName { get; set; } = null!;
        public string ToPhone { get; set; } = null!;
        public string ToAddress { get; set; } = null!;
        public int Weight { get; set; } // gram
        public decimal OrderValue { get; set; }
    }

    public enum ShipmentStatus
    {
        Pending,
        Confirmed,
        Delivering,
        Delivered,
        Canceled
    }

    public interface IShippingService
    {
        Task<decimal> CalculateFeeAsync(ShippingRateRequest req);
        Task<string> CreateShipmentAsync(ShipmentRequest req);
        Task<ShipmentStatus> GetStatusAsync(string providerOrderCode);
    }
} 