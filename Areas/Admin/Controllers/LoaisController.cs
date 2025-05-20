using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using MyPhamSoul.Helpper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyPhamSoul.Models;
using PagedList.Core;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace MyPhamSoul.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Employee, Admin")]
    public class LoaisController : Controller
    {
        private readonly _2025MyPhamContext _context;
        public INotyfService _notifyService { get; }

        public LoaisController(_2025MyPhamContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/Loais
        public IActionResult Index(int? page, string? TenLoai = "")
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;

            IQueryable<Loai> query = _context.Loais
                .AsNoTracking();

            if (!string.IsNullOrEmpty(TenLoai))
            {
                query = query.Where(x => x.TenLoai.Contains(TenLoai));
            }

            var lsProducts = query.OrderByDescending(x => x.MaLoai).ToList();
            PagedList<Loai> models = new PagedList<Loai>(lsProducts.AsQueryable(), pageNumber, pageSize);
     
            ViewBag.CurrentPage = pageNumber;
            ViewBag.CurrentLoai = TenLoai;


            return View(models);
        }
        public IActionResult Filtter(string? TenLoai)
        {
            var url = "/Admin/Loais?";

            if (!string.IsNullOrEmpty(TenLoai))
            {
                url += $"TenLoai={TenLoai}&";
            }

            // Loại bỏ dấu '&' cuối cùng nếu có
            if (url.EndsWith("&"))
            {
                url = url.Substring(0, url.Length - 1);
            }

            return Json(new { status = "success", redirectUrl = url });
        }
        // GET: Admin/Loais/Details/5
        public async Task<IActionResult> Details(string id, int? page,string? TenLoai)
        {
            if (id == null || _context.Loais == null)
            {
                return NotFound();
            }

            var loai = await _context.Loais
                .FirstOrDefaultAsync(m => m.MaLoai == id);
            if (loai == null)
            {
                return NotFound();
            }
            ViewBag.CurrentPage = page;
            ViewBag.CurrentLoai = TenLoai;
            return View(loai);
        }
        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DetailsConfirmed(string id, int? page, string? TenLoai)
        {
            if (_context.SanPhams == null)
            {
                return Problem("Entity set '_2025MyPhamContext.SanPhams' is null.");
            }
            // Sau khi xóa thành công, bạn có thể chuyển hướng trở lại trang chứa sản phẩm vừa xóa bằng cách truyền tham số `page`.
            // Nếu `page` không có giá trị, bạn có thể mặc định nó về một trang cụ thể (ví dụ: 1).
            return RedirectToAction("Index", new { page = page, TenLoai = TenLoai});

        }
        // GET: Admin/Loais/Create
        public IActionResult Create(int? page, string? TenLoai)
        {
          
            return View();
        }

        // POST: Admin/Loais/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? page, string? TenLoai, [Bind("MaLoai,TenLoai")] Loai loai)
        {
            // Kiểm tra xem mã loại đã tồn tại chưa
            if (_context.Loais.Any(l => l.MaLoai == loai.MaLoai))
            {
                ModelState.AddModelError("MaLoai", "Mã loại sản phẩm đã tồn tại.");
            }

            if (string.IsNullOrEmpty(loai.MaLoai))
            {
                ModelState.AddModelError("MaLoai", "Mã loại sản phẩm là trường bắt buộc.");
            }

            if (string.IsNullOrEmpty(loai.TenLoai))
            {
                ModelState.AddModelError("TenLoai", "Xin hãy nhập tên của loại sản phẩm.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(loai);
                await _context.SaveChangesAsync();
                _notifyService.Success("Tạo mới sản phẩm thành công");
                return RedirectToAction("Index", new { page = page, TenLoai = TenLoai });
            }
            return View(loai);
        }

        // GET: Admin/Loais/Edit/5
        public async Task<IActionResult> Edit(string id, int? page, string? TenLoai)
        {
            if (id == null || _context.Loais == null)
            {
                return NotFound();
            }

            var loai = await _context.Loais.FindAsync(id);
            if (loai == null)
            {
                return NotFound();
            }
            ViewBag.CurrentPage = page;
            ViewBag.CurrentLoai = TenLoai;
            return View(loai);
        }

        // POST: Admin/Loais/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, int? page, string? TenLoai, [Bind("MaLoai,TenLoai")] Loai loai)
        {
            if (id != loai.MaLoai)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loai);
                    await _context.SaveChangesAsync();
                    _notifyService.Success("Cập nhật sản phẩm thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoaiExists(loai.MaLoai))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", new { page = page, TenLoai = TenLoai });
            }
            return View(loai);
        }

        // GET: Admin/Loais/Delete/5
        public async Task<IActionResult> Delete(string id, int? page, string? TenLoai)
        {
            if (id == null || _context.Loais == null)
            {
                return NotFound();
            }

            var loai = await _context.Loais
                .FirstOrDefaultAsync(m => m.MaLoai == id);
            if (loai == null)
            {
                return NotFound();
            }
            ViewBag.CurrentPage = page;
            ViewBag.CurrentLoai = TenLoai;
            return View(loai);
        }

        // POST: Admin/Loais/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id, int? page, string? TenLoai)
        {
            if (_context.Loais == null)
            {
                return Problem("Entity set '_2025MyPhamContext.Loais'  is null.");
            }
            var loai = await _context.Loais.FindAsync(id);
            if (loai == null)
            {
                return NotFound();
            }
            // Xác nhận không có chi tiết loại nào liên quan
            var ctloai = await _context.Ctloais.Where(p => p.MaLoai == id).ToListAsync();

            // Xác nhận không có danh mục sản phẩm nào liên quan

            var danhMuc = _context.DanhMucSanPhams
     .AsEnumerable()
     .Where(ct => ctloai.Any(p => p.MaCtloai == ct.MaCtloai))
     .ToList();
            // Xác nhận không có sản phẩm nào liên quan
            var products = _context.SanPhams
                             .AsEnumerable()
                            .Where(ct => danhMuc.Any(p => p.MaDanhMuc == ct.MaDanhMuc))
                            .ToList();
            // Xác nhận không có chi tiết đơn hàng nào liên quan
            var orderDetails = _context.ChiTietDonHangs
    .AsEnumerable()
    .Where(ct => products.Any(p => p.MaSanPham == ct.MaSanPham))
    .ToList();

            // Xóa chi tiết đơn hàng
            _context.RemoveRange(orderDetails);

            // Xóa sản phẩm
            _context.SanPhams.RemoveRange(products);
            _context.DanhMucSanPhams.RemoveRange(danhMuc);
            _context.Ctloais.RemoveRange(ctloai);
            // Xóa Hãng sản phẩm
            _context.Loais.Remove(loai);

            await _context.SaveChangesAsync();
            _notifyService.Success("Xóa loại sản phẩm thành công");
        
            return RedirectToAction("Index", new { page = page, TenLoai = TenLoai });
        }

        private bool LoaiExists(string id)
        {
          return (_context.Loais?.Any(e => e.MaLoai == id)).GetValueOrDefault();
        }
    }
}
