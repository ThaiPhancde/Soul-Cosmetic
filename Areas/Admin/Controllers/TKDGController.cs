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
    public class TKDGController : Controller
    {
        private readonly _2025MyPhamContext _context;
        public TKDGController(_2025MyPhamContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public record DailyRevenueDTO(DateTime Date, int Diem5, int Diem4, int Diem3, int Diem2, int Diem1);
        public record MonthlyRevenueDTO(DateTime Month, int Diem5, int Diem4, int Diem3, int Diem2, int Diem1);

        public record YearlyRevenueDTO(int year, int Diem5, int Diem4, int Diem3, int Diem2, int Diem1);
        [HttpGet]
        public IActionResult ThongKeDH(DateTime startDate, DateTime endDate)
        {
            try
            {
                var result = new List<DailyRevenueDTO>();

                for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                {
                    // Tính tổng đơn hàng đã hoàn thành
                    var tongDiem5 = _context.DanhGias
                        .Count(o => o.Diem == 5 && o.NgayDG.Date == date.Date);

                    // Tính tổng đơn hàng đã hủy
                    var tongDiem4 = _context.DanhGias
                        .Count(o => o.Diem == 4 && o.NgayDG.Date == date.Date);

                    var tongDiem3 = _context.DanhGias
                        .Count(o => o.Diem == 3 && o.NgayDG.Date == date.Date);

                    var tongDiem2 = _context.DanhGias
                        .Count(o => o.Diem == 2 && o.NgayDG.Date == date.Date);

                    var tongDiem1 = _context.DanhGias
                        .Count(o => o.Diem == 1 && o.NgayDG.Date == date.Date);

                    var dailyResult = new DailyRevenueDTO(date, tongDiem5, tongDiem4, tongDiem3, tongDiem2, tongDiem1);
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
                    int tongDiem5 = _context.DanhGias
                        .Where(o => o.Diem == 5 && o.NgayDG.Date >= firstDayOfMonth && o.NgayDG.Date <= lastDayOfMonth)
                        .Count();

                    // Tính tổng giá trị đơn hàng (DoanhThu)
                    int tongDiem4 = _context.DanhGias
                       .Where(o => o.Diem == 4 && o.NgayDG.Date >= firstDayOfMonth && o.NgayDG.Date <= lastDayOfMonth)
                       .Count();

                    int tongDiem3 = _context.DanhGias
                       .Where(o => o.Diem == 3 && o.NgayDG.Date >= firstDayOfMonth && o.NgayDG.Date <= lastDayOfMonth)
                       .Count();

                    int tongDiem2 = _context.DanhGias
                       .Where(o => o.Diem == 2 && o.NgayDG.Date >= firstDayOfMonth && o.NgayDG.Date <= lastDayOfMonth)
                       .Count();

                    int tongDiem1 = _context.DanhGias
                       .Where(o => o.Diem == 1 && o.NgayDG.Date >= firstDayOfMonth && o.NgayDG.Date <= lastDayOfMonth)
                       .Count();


                                 var monthlyResult = new MonthlyRevenueDTO(firstDayOfMonth, tongDiem5, tongDiem4, tongDiem3, tongDiem2, tongDiem1);

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
                    var tongDiem5 = _context.DanhGias
                        .Where(o => o.Diem == 5 && o.NgayDG.Year == year)
                        .Count();

                    var tongDiem4 = _context.DanhGias
                        .Where(o => o.Diem == 4 && o.NgayDG.Year == year)
                        .Count();

                    var tongDiem3 = _context.DanhGias
                        .Where(o => o.Diem == 3 && o.NgayDG.Year == year)
                        .Count();

                    var tongDiem2 = _context.DanhGias
                        .Where(o => o.Diem == 2 && o.NgayDG.Year == year)
                        .Count();

                    var tongDiem1 = _context.DanhGias
                        .Where(o => o.Diem == 1 && o.NgayDG.Year == year)
                        .Count();
                    
                    var yearlyResult = new YearlyRevenueDTO(year, tongDiem5, tongDiem4, tongDiem3, tongDiem2, tongDiem1);

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
