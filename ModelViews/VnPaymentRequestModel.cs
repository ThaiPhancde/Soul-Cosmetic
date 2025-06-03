using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPhamSoul.ModelViews
{
    public class VnPaymentRequestModel
    {
        public string OrderId { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string FullName { get; set; }
        public string CustomerId { get; set; }
    }
} 