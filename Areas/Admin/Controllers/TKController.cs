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
    public class TKController : Controller
    {
        private readonly _2025MyPhamContext _context;
        public TKController(_2025MyPhamContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public record DailyRevenueDTO(DateTime Date, double DoanhThu, double LoiNhuan);
        public record MonthlyRevenueDTO(DateTime Month, double DoanhThu, double LoiNhuan);

        public record YearlyRevenueDTO(int year, double DoanhThu, double LoiNhuan);
        [HttpGet]
        public IActionResult ThongKeDoanhThu(DateTime startDate, DateTime endDate)
        {
            try
            {
                var result = new List<DailyRevenueDTO>();

                for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                {
                    // Tính tổng giá trị sản phẩm trong đơn hàng
                    var totalProductValue = _context.DonHangs
                        .Where(o => o.TrangThaiDonHang == 3 && o.NgayDatHang.Value.Date == date.Date)
                        .SelectMany(o => o.ChiTietDonHangs)
                        .Join(_context.SanPhams, od => od.MaSanPham, p => p.MaSanPham, (od, p) => new { od, p })
                        .Sum(x => (double?)x.od.SoLuong * x.p.GiaNhap) ?? 0;

                    // Tính tổng giá trị đơn hàng (DoanhThu)
                    var totalAmount = _context.DonHangs
                        .Where(o => o.TrangThaiDonHang == 3 && o.NgayDatHang.Value.Date == date.Date)
                        .Sum(o => (double?)o.TongTien) ?? 0;

                    // Tính lợi nhuận
                    var profit = totalAmount - totalProductValue;

                    var dailyResult = new DailyRevenueDTO(date, totalAmount, profit);

                    result.Add(dailyResult);
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { Error = "Lỗi trong quá trình xử lý yêu cầu: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult ThongKeDoanhThuThang(DateTime startMonth, DateTime endMonth)
        {
            try
            {
                var result = new List<MonthlyRevenueDTO>();

                // Lặp qua từng tháng từ startMonth đến endMonth
                for (DateTime month = startMonth.Date; month <= endMonth.Date; month = month.AddMonths(1))
                {
                    // Tính ngày đầu tiên và ngày cuối cùng của tháng
                    var firstDayOfMonth = new DateTime(month.Year, month.Month, 1);
                    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                    // Tính tổng giá trị sản phẩm trong đơn hàng
                    var totalProductValue = _context.DonHangs
                        .Where(o => o.TrangThaiDonHang == 3 && o.NgayDatHang.Value.Date >= firstDayOfMonth && o.NgayDatHang.Value.Date <= lastDayOfMonth)
                        .SelectMany(o => o.ChiTietDonHangs)
                        .Join(_context.SanPhams, od => od.MaSanPham, p => p.MaSanPham, (od, p) => new { od, p })
                        .Sum(x => (double?)x.od.SoLuong * x.p.GiaNhap) ?? 0;

                    // Tính tổng giá trị đơn hàng (DoanhThu)
                    var totalAmount = _context.DonHangs
                        .Where(o => o.TrangThaiDonHang == 3 && o.NgayDatHang.Value.Date >= firstDayOfMonth && o.NgayDatHang.Value.Date <= lastDayOfMonth)
                        .Sum(o => (double?)o.TongTien) ?? 0;

                    // Tính lợi nhuận
                    var profit = totalAmount - totalProductValue;

                    var monthlyResult = new MonthlyRevenueDTO(firstDayOfMonth, totalAmount, profit);

                    result.Add(monthlyResult);
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { Error = "Lỗi trong quá trình xử lý yêu cầu: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult ThongKeDoanhThuNam(int startYear, int endYear)
        {
            try
            {
                var result = new List<YearlyRevenueDTO>();

                for (int year = startYear; year <= endYear; year++)
                {
                    // Tính tổng doanh thu và lợi nhuận cho từng năm
                    var totalRevenue = _context.DonHangs
                        .Where(o => o.TrangThaiDonHang == 3 && o.NgayDatHang.Value.Year == year)
                        .Sum(o => (double?)o.TongTien) ?? 0;

                    var totalProductValue = _context.DonHangs
                        .Where(o => o.TrangThaiDonHang == 3 && o.NgayDatHang.Value.Year == year)
                        .SelectMany(o => o.ChiTietDonHangs)
                        .Join(_context.SanPhams, od => od.MaSanPham, p => p.MaSanPham, (od, p) => new { od, p })
                        .Sum(x => (double?)x.od.SoLuong * x.p.GiaNhap) ?? 0;

                    var totalProfit = totalRevenue - totalProductValue;

                    var yearlyResult = new YearlyRevenueDTO(year, totalRevenue, totalProfit);

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
