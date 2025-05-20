using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPhamSoul.Models;
using System.Web.WebPages;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System.Data;
using Microsoft.AspNetCore.Authorization;


namespace MyPhamSoul.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Employee,Admin")]
    public class TKSPController : Controller
    {
        private readonly _2025MyPhamContext _context;
        public TKSPController(_2025MyPhamContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public class DailyRevenueDTO
        {
            public string ProductName { get; set; }
            public int? QuantitySold { get; set; } // Thay đổi kiểu dữ liệu thành int

            public DailyRevenueDTO(string productName, int? quantitySold)
            {
                ProductName = productName;
                QuantitySold = quantitySold;
            }
        }

        [HttpGet]
        public IActionResult ThongKeDH(DateTime startDate, DateTime endDate)
        {
            try
            {
                var result = new List<DailyRevenueDTO>();

                // Lấy danh sách sản phẩm và số lượng bán của từng sản phẩm trong khoảng thời gian startDate và endDate
                var productSales = _context.ChiTietDonHangs
                    .Where(ct => ct.MaDonHangNavigation.NgayDatHang.Value.Date >= startDate.Date && ct.MaDonHangNavigation.NgayDatHang.Value.Date <= endDate.Date && ct.MaDonHangNavigation.TrangThaiDonHang == 3)
                    .GroupBy(ct => ct.MaSanPham)
                    .Select(g => new {
                        ProductName = g.Select(ct => ct.MaSanPhamNavigation.TenSanPham).FirstOrDefault(),
                        QuantitySold = g.Sum(ct => ct.SoLuong)
                    })
                    .OrderByDescending(g => g.QuantitySold)
                    .ThenBy(g => g.ProductName) // Sắp xếp theo tên sản phẩm để đảm bảo sự ổn định khi chọn các sản phẩm cùng số lượng bán
                    .Take(10) // Chỉ lấy 10 sản phẩm đầu tiên
                    .ToList();

                // Duyệt qua danh sách sản phẩm đã sắp xếp
                foreach (var productSale in productSales)
                {
                    // Tạo một DailyRevenueDTO mới với tên sản phẩm và số lượng đã bán
                    var dailyResult = new DailyRevenueDTO(productSale.ProductName, productSale.QuantitySold);

                    // Thêm vào danh sách kết quả
                    result.Add(dailyResult);
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { Error = "Lỗi trong quá trình xử lý yêu cầu: " + ex.Message });
            }
        }


        public class MonthlyRevenueDTO
        {
            public string ProductName { get; set; }
            public int? QuantitySold { get; set; } // Thay đổi kiểu dữ liệu thành int

            public MonthlyRevenueDTO(string productName, int? quantitySold)
            {
                ProductName = productName;
                QuantitySold = quantitySold;
            }
        }

        [HttpGet]
        public IActionResult ThongKeDHThang(DateTime startMonth, DateTime endMonth)
        {
            try
            {
                var result = new List<MonthlyRevenueDTO>();

                // Tính toán tổng số lượng bán của mỗi sản phẩm trong khoảng thời gian từ ngày đầu tiên của startMonth đến ngày cuối cùng của endMonth
                var productSales = _context.ChiTietDonHangs
                    .Where(ct => ct.MaDonHangNavigation.NgayDatHang.Value.Date >= startMonth.Date && ct.MaDonHangNavigation.NgayDatHang.Value.Date <= endMonth.AddMonths(1).AddDays(-1).Date && ct.MaDonHangNavigation.TrangThaiDonHang == 3)
                    .GroupBy(ct => ct.MaSanPham)
                    .Select(g => new {
                        ProductId = g.Key,
                        ProductName = g.Select(ct => ct.MaSanPhamNavigation.TenSanPham).FirstOrDefault(),
                        QuantitySold = g.Sum(ct => ct.SoLuong)
                    })
                    .OrderByDescending(g => g.QuantitySold)
                    .ThenBy(g => g.ProductId)
                    .Take(10) // Chỉ lấy 5 sản phẩm đầu tiên
                    .ToList();

                // Duyệt qua danh sách sản phẩm đã sắp xếp
                foreach (var productSale in productSales)
                {
                    // Tạo một MonthlyRevenueDTO mới với tên sản phẩm và tổng số lượng đã bán
                    var monthlyResult = new MonthlyRevenueDTO(productSale.ProductName, productSale.QuantitySold);

                    // Thêm vào danh sách kết quả
                    result.Add(monthlyResult);
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { Error = "Lỗi trong quá trình xử lý yêu cầu: " + ex.Message });
            }
        }


        public class YearlyRevenueDTO
        {
            public string ProductName { get; set; }
            public int? QuantitySold { get; set; } // Thay đổi kiểu dữ liệu thành int

            public YearlyRevenueDTO(string productName, int? quantitySold)
            {
                ProductName = productName;
                QuantitySold = quantitySold;
            }
        }


        [HttpGet]
        public IActionResult ThongKeDHNam(int startYear, int endYear)
        {
            try
            {
                var result = new List<YearlyRevenueDTO>();

                // Tính toán tổng số lượng bán của mỗi sản phẩm trong khoảng thời gian từ ngày đầu tiên của startYear đến ngày cuối cùng của endYear
                var startDate = new DateTime(startYear, 1, 1);
                var endDate = new DateTime(endYear, 12, 31);

                var productSales = _context.ChiTietDonHangs
                    .Where(ct => ct.MaDonHangNavigation.NgayDatHang.Value.Date >= startDate.Date && ct.MaDonHangNavigation.NgayDatHang.Value.Date <= endDate.Date && ct.MaDonHangNavigation.TrangThaiDonHang == 3)
                    .GroupBy(ct => ct.MaSanPham)
                    .Select(g => new {
                        ProductId = g.Key,
                        ProductName = g.Select(ct => ct.MaSanPhamNavigation.TenSanPham).FirstOrDefault(),
                        QuantitySold = g.Sum(ct => ct.SoLuong)
                    })
                    .OrderByDescending(g => g.QuantitySold)
                    .ThenBy(g => g.ProductId)
                    .Take(10) // Chỉ lấy 10 sản phẩm đầu tiên
                    .ToList();

                // Duyệt qua danh sách sản phẩm đã sắp xếp
                foreach (var productSale in productSales)
                {
                    // Tạo một YearlyRevenueDTO mới với tên sản phẩm và tổng số lượng đã bán
                    var yearlyResult = new YearlyRevenueDTO(productSale.ProductName, productSale.QuantitySold);

                    // Thêm vào danh sách kết quả
                    result.Add(yearlyResult);
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { Error = "Lỗi trong quá trình xử lý yêu cầu: " + ex.Message });
            }
        }

    }
}
