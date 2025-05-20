using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyPhamSoul.Helpper;
using MyPhamSoul.Models;
using MyPhamSoul.ModelViews;
using PagedList.Core;


namespace MyPhamSoul.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoHangs1Controller : Controller
    {
        private readonly _2025MyPhamContext _context;

        public INotyfService _notifyService { get; }

        public LoHangs1Controller(_2025MyPhamContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/LoHangs

        public IActionResult Index(int? page, string? MaID = null, string search = "", DateTime? startDate = null, DateTime? endDate = null)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;

            IQueryable<LoHang> query = _context.LoHangs
                .AsNoTracking().Include(dh => dh.ChiTietLoHangs).ThenInclude(c => c.MaSanPhamNavigation)
                  .Include(x => x.MaNhaPpNavigation)
                  .Where(lh => lh.TrangThaiLH == 2);



            if (!string.IsNullOrEmpty(MaID))
            {
                query = query.Where(x => x.MaLoHang.Contains(MaID));
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.MaNhaPpNavigation.TenNhaPp.Contains(search));
            }

            if (startDate != null)
            {
                query = query.Where(x => x.NgayNhan.Value.Date >= startDate.Value.Date);
            }

            if (endDate != null)
            {
                query = query.Where(x => x.NgayNhan.Value.Date <= endDate.Value.Date);
            }



            var lsDonHangs = query.OrderByDescending(x => x.MaLoHang).ToList();

            PagedList<LoHang> models = new PagedList<LoHang>(lsDonHangs.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentMaID = MaID;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentstartDate = startDate;
            ViewBag.CurrentendDate = endDate;

            //ViewData["KhachHang"] = new SelectList(_context.DanhMucSanPhams, "MaKhachHang", "TenKhachHang", MaKH);

            return View(models);
        }
        // GET: Admin/LoHangs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.LoHangs == null)
            {
                return NotFound();
            }

            var loHang = await _context.LoHangs
                .AsNoTracking().Include(dh => dh.ChiTietLoHangs).ThenInclude(c => c.MaSanPhamNavigation)
                  .Include(x => x.MaNhaPpNavigation)
                .FirstOrDefaultAsync(m => m.MaLoHang == id);
            if (loHang == null)
            {
                return NotFound();
            }
            var lohang = _context.LoHangs
               .AsNoTracking().Include(dh => dh.ChiTietLoHangs).ThenInclude(c => c.MaSanPhamNavigation)
                 .Include(x => x.MaNhaPpNavigation)
               .Where(x => x.MaLoHang == loHang.MaLoHang)
                .OrderBy(x => x.MaLoHang).ToList();
            ViewBag.ChiTiet = lohang;

            return View(loHang);
        }
        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]

        public IActionResult Filtter(string? MaID = null, string search = "", DateTime? startDate = null, DateTime? endDate = null)
        {
            var url = "/Admin/LoHangs?";
            if (!string.IsNullOrEmpty(MaID))
            {
                url += $"MaID={MaID}&";
            }

            if (!string.IsNullOrEmpty(search))
            {
                url += $"search={search}&";
            }

            if (startDate != null)
            {
                url += $"startDate={startDate:yyyy-MM-dd}&"; // Đảm bảo định dạng ngày là yyyy-MM-dd
            }
            if (endDate != null)
            {
                url += $"endDate={endDate:yyyy-MM-dd}&"; // Đảm bảo định dạng ngày là yyyy-MM-dd
            }

            // Loại bỏ dấu '&' cuối cùng nếu có
            if (url.EndsWith("&"))
            {
                url = url.Substring(0, url.Length - 1);
            }

            return Json(new { status = "success", redirectUrl = url });
        }
    }
}