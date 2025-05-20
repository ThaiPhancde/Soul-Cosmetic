using System.Collections.Generic;

namespace MyPhamSoul.Models
{
    public partial class DonHang
    {
        public virtual ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    }
} 