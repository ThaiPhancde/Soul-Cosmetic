using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Helpers;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyPhamSoul.ChangPassword;
using MyPhamSoul.Extension;
using MyPhamSoul.Helpper;
using MyPhamSoul.Models;
using Newtonsoft.Json.Linq;
using PagedList.Core;
using static System.Net.Mime.MediaTypeNames;

namespace MyPhamSoul.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Employee,Admin")]
    public class DanhGiaController : Controller
    {
        private readonly _2025MyPhamContext _context;
        public INotyfService _notifyService { get; }

        public DanhGiaController(_2025MyPhamContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }
        public IActionResult Index(int? page, string? MaID = null, string search = "", DateTime? startDate = null, DateTime? endDate = null, int? diem = null)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;

            IQueryable<DanhGia> query = _context.DanhGias
                .AsNoTracking();
                

            if (!string.IsNullOrEmpty(MaID))
            {
                query = query.Where(x => x.MaDanhMuc.Contains(MaID));
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.NoiDung.Contains(search));
            }

            if (startDate != null)
            {
                query = query.Where(x => x.NgayDG.Date >= startDate.Value.Date);
            }

            if (endDate != null)
            {
                query = query.Where(x => x.NgayDG.Date <= endDate.Value.Date);
            }

            

            if (diem != null)
            {
                query = query.Where(x => x.Diem == diem);
            }

            var lsDonHangs = query.OrderByDescending(x => x.NgayDG).ToList();

            PagedList<DanhGia> models = new PagedList<DanhGia>(lsDonHangs.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentMaID = MaID;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentstartDate = startDate;
            ViewBag.CurrentendDate = endDate;
            
            ViewBag.CurrentendDiem = diem;
            List<SelectListItem> lsDiem = new List<SelectListItem>();
            lsDiem.Add(new SelectListItem() { Text = "5 sao", Value = "5" });
            lsDiem.Add(new SelectListItem() { Text = "4 sao", Value = "4" });
            lsDiem.Add(new SelectListItem() { Text = "3 sao", Value = "3" });
            lsDiem.Add(new SelectListItem() { Text = "2 sao", Value = "2" });
            lsDiem.Add(new SelectListItem() { Text = "1 sao", Value = "1" });
            ViewData["lsDiem"] = lsDiem;
            //ViewData["KhachHang"] = new SelectList(_context.DanhMucSanPhams, "MaKhachHang", "TenKhachHang", MaKH);

            return View(models);
        }

        public IActionResult Filtter(string? MaID = null, string search = "", DateTime? startDate = null, DateTime? endDate = null, int? diem = null)
        {
            var url = "/Admin/DanhGia?";
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



            if (diem != null)
            {
                url += $"diem={diem}&";
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
