using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyPhamSoul.Helpper;
using MyPhamSoul.Models;
using PagedList.Core;

namespace MyPhamSoul.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Employee, Admin")]
    public class DanhMucSanPhamsController : Controller
    {
        private readonly _2025MyPhamContext _context;

        public INotyfService _notifyService { get; }

        public DanhMucSanPhamsController(_2025MyPhamContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/DanhMucSanPhams
        public IActionResult Index(int? page, string? MaID = "", string search = "", double? minPrice = null, double? maxPrice = null)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 5;
            IQueryable<DanhMucSanPham> query = _context.DanhMucSanPhams
               .AsNoTracking()
               .Include(x => x.MaHangNavigation)
               .Include(x => x.MaCtloaiNavigation);


            if (!string.IsNullOrEmpty(MaID))
            {
                query = query.Where(x => x.MaDanhMuc.Contains(MaID));
            }


            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.TenDanhMuc.Contains(search));
            }

            if (minPrice != null)
            {
                query = query.Where(x => x.Gia >= minPrice);
            }

            if (maxPrice != null)
            {
                query = query.Where(x => x.Gia <= maxPrice);
            }

            var lsCategorys = query.OrderByDescending(x => x.Gia).ToList();
            PagedList<DanhMucSanPham> models = new PagedList<DanhMucSanPham>(lsCategorys.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentMaID = MaID;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;
          
            return View(models);
        }

        public IActionResult Filtter( string search, double? minPrice, double? maxPrice, string? MaID)
        {
            var url = "/Admin/DanhMucSanPhams?";
            if (MaID != null)
            {
                url += $"MaID={MaID}&";
            }
        
            if (!string.IsNullOrEmpty(search))
            {
                url += $"search={search}&";
            }

            if (minPrice != null)
            {
                url += $"minPrice={minPrice}&";
            }

            if (maxPrice != null)
            {
                url += $"maxPrice={maxPrice}&";
            }

            // Loại bỏ dấu '&' cuối cùng nếu có
            if (url.EndsWith("&"))
            {
                url = url.Substring(0, url.Length - 1);
            }

            return Json(new { status = "success", redirectUrl = url });
        }


        // GET: Admin/DanhMucSanPhams/Details/5
        public async Task<IActionResult> Details(string id, int? page, string? MaID, string? search, string? MaDM, double? minPrice, double? maxPrice)
        {
            if (id == null || _context.DanhMucSanPhams == null)
            {
                return NotFound();
            }

            var danhMucSanPham = await _context.DanhMucSanPhams
                .Include(d => d.MaCtloaiNavigation)
                .Include(d => d.MaHangNavigation)
                .FirstOrDefaultAsync(m => m.MaDanhMuc == id);
            if (danhMucSanPham == null)
            {
                return NotFound();
            }
            ViewBag.CurrentPage = page;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentMaID = MaID;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;
            return View(danhMucSanPham);
        }
        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DetailsConfirmed(string id, int? page, string? MaID, string? search, double? minPrice, double? maxPrice)
        {
            if (_context.DanhMucSanPhams == null)
            {
                return Problem("Entity set '_2025MyPhamContext.DanhMucSanPhams' is null.");
            }
            // Sau khi xóa thành công, bạn có thể chuyển hướng trở lại trang chứa sản phẩm vừa xóa bằng cách truyền tham số `page`.
            // Nếu `page` không có giá trị, bạn có thể mặc định nó về một trang cụ thể (ví dụ: 1).
            return RedirectToAction("Index", new { page = page, MaID = MaID, search = search, minPrice = minPrice, maxPrice = maxPrice });

        }
        // GET: Admin/DanhMucSanPhams/Create
        public IActionResult Create(int? page, string? search, string? MaID, double? minPrice, double? maxPrice)
        {
            ViewData["Loai"] = new SelectList(_context.Ctloais, "MaCtloai", "TenCtloai");
            ViewData["Hang"] = new SelectList(_context.Hangs, "MaHang", "TenHang");
            ViewBag.CurrentPage = page;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentMaID = MaID;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;
            return View();
        }

        // POST: Admin/DanhMucSanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? page, [Bind("MaDanhMuc,TenDanhMuc,MoTa,MaCtloai,MaHang,HinhAnh,DanhGia,Gia,CachDung,ChiTiet")] DanhMucSanPham danhMucSanPham, IFormFile? fAnh)
        {
            if (string.IsNullOrEmpty(danhMucSanPham.MaDanhMuc))
            {
                ModelState.AddModelError("MaDanhMuc", "Mã danh mục là trường bắt buộc.");
            }

            if (string.IsNullOrEmpty(danhMucSanPham.TenDanhMuc))
            {
                ModelState.AddModelError("TenDanhMuc", "Xin hãy nhập tên của danh mục sản phẩm.");
            }

            if (danhMucSanPham.Gia == null)
            {
                ModelState.AddModelError("Gia", "Xin hãy nhập giá bán của danh mục sản phẩm.");
            }

            if (danhMucSanPham.MaCtloai == null)
            {
                ModelState.AddModelError("MaCtloai", "Xin hãy chọn loại sản phẩm.");
            }
            if (danhMucSanPham.MaHang == null)
            {
                ModelState.AddModelError("MaHang", "Xin hãy chọn hãng sản phẩm.");
            }

            if (ModelState.IsValid)
            {
                if (_context.DanhMucSanPhams.Any(x => x.MaDanhMuc == danhMucSanPham.MaDanhMuc))
                {
                    ModelState.AddModelError("MaDanhMuc", "Mã danh mục sản phẩm đã tồn tại. Vui lòng chọn mã khác.");
                    ViewData["DanhMuc"] = new SelectList(_context.DanhMucSanPhams, "MaDanhMuc", "TenDanhMuc", danhMucSanPham.MaDanhMuc);
                    return View(danhMucSanPham);
                }

                danhMucSanPham.TenDanhMuc = Utilities.ToTitleCase(danhMucSanPham.TenDanhMuc);
                if (fAnh != null)
                {
                    string extention = Path.GetExtension(fAnh.FileName);
                    string image = Utilities.SEOUrl(danhMucSanPham.TenDanhMuc) + extention;
                    danhMucSanPham.HinhAnh = await Utilities.UploadFile(fAnh, @"DanhMucSanPham", image.ToLower());
                }
                _context.Add(danhMucSanPham);
                await _context.SaveChangesAsync();
                _notifyService.Success("Tạo mới danh mục sản phẩm thành công");
                return RedirectToAction("Index", new { page = page });
            }
            ViewData["Loai"] = new SelectList(_context.Ctloais, "MaCtloai", "TenCtloai", danhMucSanPham.MaCtloai);
            ViewData["Hang"] = new SelectList(_context.Hangs, "MaHang", "TenHang", danhMucSanPham.MaHang);
            return View(danhMucSanPham);
        }

        // GET: Admin/DanhMucSanPhams/Edit/5
        public async Task<IActionResult> Edit(string id, int? page, string? MaID, string? search, double? minPrice, double? maxPrice)
        {

            if (id == null || _context.DanhMucSanPhams == null)
            {
                return NotFound();
            }

            var danhMucSanPham = await _context.DanhMucSanPhams.FindAsync(id);
            if (danhMucSanPham == null)
            {
                return NotFound();
            }
            ViewBag.CurrentPage = page;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentMaID = MaID;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;
            ViewData["Loai"] = new SelectList(_context.Ctloais, "MaCtloai", "TenCtloai", danhMucSanPham.MaCtloai);
            ViewData["Hang"] = new SelectList(_context.Hangs, "MaHang", "TenHang", danhMucSanPham.MaHang);
            return View(danhMucSanPham);
        }

        // POST: Admin/DanhMucSanPhams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, int? page, string? search, string? MaID, double? minPrice, double? maxPrice ,[Bind("MaDanhMuc,TenDanhMuc,MoTa,MaCtloai,MaHang,HinhAnh,DanhGia,Gia,CachDung,ChiTiet")] DanhMucSanPham danhMucSanPham, IFormFile? fAnh)
        {

            if (id != danhMucSanPham.MaDanhMuc)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    danhMucSanPham.TenDanhMuc = Utilities.ToTitleCase(danhMucSanPham.TenDanhMuc);

                    if (fAnh != null)
                    {
                        // Có tệp hình ảnh mới được chọn
                        string extention = Path.GetExtension(fAnh.FileName);
                        string image = Utilities.SEOUrl(danhMucSanPham.TenDanhMuc) + extention;
                        danhMucSanPham.HinhAnh = await Utilities.UploadFile(fAnh, @"sanPhams", image.ToLower());
                    }

                    if (string.IsNullOrEmpty(danhMucSanPham.HinhAnh))
                    {
                        // Nếu không có hình ảnh mới, giữ nguyên hình ảnh hiện tại
                        danhMucSanPham.HinhAnh = "default.jpg";
                    }
                   
                    _context.Update(danhMucSanPham);
                    await _context.SaveChangesAsync();
                    _notifyService.Success("Cập nhật danh mục sản phẩm thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DanhMucSanPhamExists(danhMucSanPham.MaDanhMuc))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", new { page = page, MaID = MaID, search = search, minPrice = minPrice, maxPrice = maxPrice });
            }
            ViewData["Loai"] = new SelectList(_context.Ctloais, "MaCtloai", "TenCtloai", danhMucSanPham.MaCtloai);
            ViewData["Hang"] = new SelectList(_context.Hangs, "MaHang", "TenHang", danhMucSanPham.MaHang);
            return View(danhMucSanPham);
        }

        // GET: Admin/DanhMucSanPhams/Delete/5
        public async Task<IActionResult> Delete(string id, int? page, string? MaID, string? search, double? minPrice, double? maxPrice)
        {
            if (id == null || _context.DanhMucSanPhams == null)
            {
                return NotFound();
            }

            var danhMucSanPham = await _context.DanhMucSanPhams
                .Include(d => d.MaCtloaiNavigation)
                .Include(d => d.MaHangNavigation)
                .FirstOrDefaultAsync(m => m.MaDanhMuc == id);
            if (danhMucSanPham == null)
            {
                return NotFound();
            }
            ViewBag.CurrentPage = page;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentMaID = MaID;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;
            return View(danhMucSanPham);
        }

        // POST: Admin/DanhMucSanPhams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        // POST: Admin/Hang/Delete/5

        public async Task<IActionResult> DeleteConfirmed(string id, int? page, string? MaID, string? search, double? minPrice, double? maxPrice)
        {
            var danhMuc = await _context.DanhMucSanPhams.FindAsync(id);
            if (danhMuc == null)
            {
                return NotFound();
            }

            // Xác nhận không có sản phẩm nào liên quan
            var products = await _context.SanPhams.Where(p => p.MaDanhMuc == id).ToListAsync();

            // Xác nhận không có chi tiết đơn hàng nào liên quan
            var orderDetails = _context.ChiTietDonHangs
    .AsEnumerable()
    .Where(ct => products.Any(p => p.MaSanPham == ct.MaSanPham))
    .ToList();

            // Xóa chi tiết đơn hàng
            _context.RemoveRange(orderDetails);

            // Xóa sản phẩm
            _context.SanPhams.RemoveRange(products);

            // Xóa Hãng sản phẩm
            _context.DanhMucSanPhams.Remove(danhMuc);

            await _context.SaveChangesAsync();
            _notifyService.Success("Xóa danh mục sản phẩm thành công");

            return RedirectToAction("Index", new { page, MaID, search, minPrice, maxPrice });
        }

        private bool DanhMucSanPhamExists(string id)
        {
          return (_context.DanhMucSanPhams?.Any(e => e.MaDanhMuc == id)).GetValueOrDefault();
        }
    }
}
