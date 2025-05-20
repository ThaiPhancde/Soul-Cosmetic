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
    public class TKDHController : Controller
    {
        private readonly _2025MyPhamContext _context;
        public TKDHController(_2025MyPhamContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public record DailyRevenueDTO(DateTime Date, int DonHangTC, int DonHangHuy);
        public record MonthlyRevenueDTO(DateTime Month, int DonHangTC, int DonHangHuy);

        public record YearlyRevenueDTO(int year, int DonHangTC, int DonHangHuy);
        [HttpGet]
        public IActionResult ThongKeDH(DateTime startDate, DateTime endDate)
        {
            try
            {
                var result = new List<DailyRevenueDTO>();

                for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                {
                    // Tính tổng đơn hàng đã hoàn thành
                    var tongDonHangTC = _context.DonHangs
                        .Count(o => o.TrangThaiDonHang == 3 && o.NgayDatHang.Value.Date == date.Date);

                    // Tính tổng đơn hàng đã hủy
                    var tongDonHangHuy = _context.DonHangs
                        .Count(o => o.TrangThaiDonHang == 4 && o.NgayDatHang.Value.Date == date.Date);

                    var dailyResult = new DailyRevenueDTO(date, tongDonHangTC, tongDonHangHuy);
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
        public IActionResult ThongKeDHThang(DateTime startMonth, DateTime endMonth)
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
                    int tongDonHangTC = _context.DonHangs
                        .Where(o => o.TrangThaiDonHang == 3 && o.NgayDatHang.Value.Date >= firstDayOfMonth && o.NgayDatHang.Value.Date <= lastDayOfMonth)
                        .Count();

                    // Tính tổng giá trị đơn hàng (DoanhThu)
                    int tongDonHangHuy = _context.DonHangs
                        .Where(o => o.TrangThaiDonHang == 4 && o.NgayDatHang.Value.Date >= firstDayOfMonth && o.NgayDatHang.Value.Date <= lastDayOfMonth)
                        .Count();

                    // Tính lợi nhuận
                    

                    var monthlyResult = new MonthlyRevenueDTO(firstDayOfMonth, tongDonHangTC, tongDonHangHuy);

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
        public IActionResult ThongKeDHNam(int startYear, int endYear)
        {
            try
            {
                var result = new List<YearlyRevenueDTO>();

                for (int year = startYear; year <= endYear; year++)
                {
                    // Tính tổng doanh thu và lợi nhuận cho từng năm
                    var tongDonHangTC = _context.DonHangs
                        .Where(o => o.TrangThaiDonHang == 3 && o.NgayDatHang.Value.Year == year)
                        .Count();

                    var tongDonHangHuy = _context.DonHangs
                        .Where(o => o.TrangThaiDonHang == 4 && o.NgayDatHang.Value.Year == year)
                        .Count();

                    

                    var yearlyResult = new YearlyRevenueDTO(year, tongDonHangTC, tongDonHangHuy);

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
