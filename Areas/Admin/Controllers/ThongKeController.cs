
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
    [Authorize(Roles = "Employee, Admin")]
    public class ThongKeController : Controller
    {

        private readonly _2025MyPhamContext _context;
        public ThongKeController(_2025MyPhamContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

       
        [HttpGet]
        public IActionResult ThongKeDoanhThu(DateTime startDate, DateTime endDate)
        {
            try
            {
                var dailyRevenues = new Dictionary<DateTime, double>();

                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    var dailyRevenue = _context.DonHangs
                        .Where(hd => hd.TrangThaiDonHang == 3 && hd.NgayDatHang.Value.Date == date.Date)
                        .Sum(hd => hd.TongTien);

                    dailyRevenues.Add(date, dailyRevenue ?? 0);

                }

                return Ok(dailyRevenues);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi trong quá trình xử lý yêu cầu: " + ex.Message);
            }
        }


        [HttpGet]
        public IActionResult ThongKeDoanhThuThang(DateTime startMonth, DateTime endMonth)
        {
            try
            {
                var monthlyRevenues = new Dictionary<string, double>();

                for (var date = startMonth; date <= endMonth; date = date.AddMonths(1))
                {
                    var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                    var monthlyRevenue = _context.DonHangs
                        .Where(hd => hd.TrangThaiDonHang == 3 && hd.NgayDatHang >= firstDayOfMonth && hd.NgayDatHang <= lastDayOfMonth)
                        .Sum(hd => hd.TongTien);

                    monthlyRevenues.Add(date.ToString("MM/yyyy"), monthlyRevenue ?? 0);
                }

                return Ok(monthlyRevenues);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi trong quá trình xử lý yêu cầu: " + ex.Message);
            }
        }

        [HttpGet]
        public IActionResult ThongKeDoanhThuNam(int startYear, int endYear)
        {
            try
            {
                var yearlyRevenues = new Dictionary<int, double>();

                for (var year = startYear; year <= endYear; year++)
                {
                    var yearlyRevenue = _context.DonHangs
                        .Where(hd => hd.TrangThaiDonHang == 3 && hd.NgayDatHang.Value.Year == year)
                        .Sum(hd => hd.TongTien);

                    yearlyRevenues.Add(year, yearlyRevenue ?? 0);
                }

                return Ok(yearlyRevenues);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi trong quá trình xử lý yêu cầu: " + ex.Message);
            }
        }



    }
}