using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.WebPages;
using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyPhamSoul.Models;
using PagedList.Core;

namespace MyPhamSoul.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Employee,Admin")]
    public class DonHangsController : Controller
    {
        private readonly _2025MyPhamContext _context;
        public INotyfService _notifyService { get; }

        public DonHangsController(_2025MyPhamContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/DonHangs
   
        public IActionResult Index(int? page, string? MaID = null, string search = "", DateTime? startDate = null, DateTime? endDate = null)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 5;

            IQueryable<DonHang> query = _context.DonHangs
                .AsNoTracking().Include(dh => dh.ChiTietDonHangs).ThenInclude(c => c.MaSanPhamNavigation)
                  .Include(x => x.MaKhachHangNavigation)
                  .Include(x => x.MaKhachHangNavigation.Account);
         
            if (!string.IsNullOrEmpty(MaID))
            {
                query = query.Where(x => x.MaDonHang.Contains(MaID));
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.MaKhachHangNavigation.TenKhachHang.Contains(search));
            }

            if (startDate != null)
            {
                query = query.Where(x => x.NgayDatHang.Value.Date >= startDate.Value.Date);
            }

            if (endDate != null)
            {
                query = query.Where(x => x.NgayDatHang.Value.Date <= endDate.Value.Date);
            }



            var lsDonHangs = query.OrderByDescending(x => x.NgayDatHang).ToList();

            PagedList<DonHang> models = new PagedList<DonHang>(lsDonHangs.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentMaID = MaID;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentstartDate = startDate;
            ViewBag.CurrentendDate = endDate;

            //ViewData["KhachHang"] = new SelectList(_context.DanhMucSanPhams, "MaKhachHang", "TenKhachHang", MaKH);

            return View(models);
        }

        public IActionResult ChiTietDonHang()
        {
            var donHangList = _context.DonHangs
                .AsNoTracking()
               .Include(dh => dh.ChiTietDonHangs)
               .ThenInclude(c => c.MaSanPhamNavigation)
                .Include(x => x.MaKhachHangNavigation)
           .ToList();

            return View(donHangList);
        }
        [HttpPost]
        public IActionResult UpdateStatus(string donHangId, int trangThaiDonHang)
        {
            var donHang = _context.ChiTietDonHangs.Find(donHangId);
            if (donHang == null)
            {
                return NotFound();
            }

            donHang.MaDonHangNavigation.TrangThaiDonHang = trangThaiDonHang;
            _context.SaveChanges();

            return Ok();
        }

        public IActionResult Filtter( string search, string? MaID, DateTime? startDate = null, DateTime? endDate = null)
        {
            var url = "/Admin/DonHangs?";
            if (MaID != null)
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


        // GET: Admin/DonHangs/Details/5
        public async Task<IActionResult> Details(string id, int? page, string? MaID, string? search, double? minPrice, double? maxPrice)
        {
            if (id == null || _context.DonHangs == null)
            {
                return NotFound();
            }

            var donHangs = await _context.DonHangs
               .AsNoTracking()
               .Include(dh => dh.ChiTietDonHangs)
               .ThenInclude(c => c.MaSanPhamNavigation)
                .Include(x => x.MaKhachHangNavigation)
                .FirstOrDefaultAsync(m => m.MaDonHang == id);
            if (donHangs == null)
            {
                return NotFound();
            }
            var donhang = _context.DonHangs
                .AsNoTracking()
                .Include(dh => dh.ChiTietDonHangs)
                .ThenInclude(c => c.MaSanPhamNavigation)
                .Include(x => x.MaKhachHangNavigation)
                .Where(x => x.MaDonHang == donHangs.MaDonHang)
                .OrderBy(x => x.MaDonHang).ToList();
            ViewBag.ChiTiet = donhang;
            ViewBag.CurrentPage = page;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;
            ViewBag.CurrentMaID = MaID;
            //string fulladdress = $"{chitietdonhang.Ma}"
            return View(donHangs);
        }
        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DetailsConfirmed(string id, int? page, string? MaID, string? search, double? minPrice, double? maxPrice)
        {
            if (_context.DonHangs == null)
            {
                return Problem("Entity set '_2025MyPhamContext.DonHangs' is null.");
            }
            // Sau khi xóa thành công, bạn có thể chuyển hướng trở lại trang chứa sản phẩm vừa xóa bằng cách truyền tham số `page`.
            // Nếu `page` không có giá trị, bạn có thể mặc định nó về một trang cụ thể (ví dụ: 1).
            return RedirectToAction("Index", new { page = page, MaID = MaID, search = search, minPrice = minPrice, maxPrice = maxPrice });

        }
        // GET: Admin/DonHangs/Create
        public IActionResult Create()
        {
            ViewData["MaKhachHang"] = new SelectList(_context.KhachHangs, "MaKhachHang", "MaKhachHang");
            return View();
        }

        // POST: Admin/DonHangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaDonHang,TongTien,TrangThaiDonHang,MaKhachHang,ThanhToan,VanChuyen,PhiVanChuyen,NgayDatHang")] DonHang donHang)
        {
            if (ModelState.IsValid)
            {
                _context.Add(donHang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaKhachHang"] = new SelectList(_context.KhachHangs, "MaKhachHang", "MaKhachHang", donHang.MaKhachHang);
            return View(donHang);
        }
        public async Task<IActionResult> ChangeStatus(string id, int? page, string? MaID, string? search, double? minPrice, double? maxPrice)
        {
            if (id == null || _context.DonHangs == null)
            {
                return NotFound();
            }

            var donHang = await _context.DonHangs
                 .AsNoTracking()
                .Include(dh => dh.ChiTietDonHangs)
                .ThenInclude(c => c.MaSanPhamNavigation)
                .Include(x => x.MaKhachHangNavigation)
                 .FirstOrDefaultAsync(m => m.MaDonHang == id);

            if (donHang == null)
            {
                return NotFound();
            }

            List<SelectListItem> trangThaiList = new List<SelectListItem>
    {
        new SelectListItem { Text = "Chờ xác nhận", Value = "1" },
        new SelectListItem { Text = "Đang vận chuyển", Value = "2" },
        new SelectListItem { Text = "Đã giao hàng", Value = "3" },
        new SelectListItem { Text = "Hủy đơn hàng", Value = "4" }
        // Thêm các trạng thái khác nếu cần
    };

            ViewData["TrangThai"] = new SelectList(trangThaiList, "Value", "Text", donHang.TrangThaiDonHang.ToString());
            ViewData["MaDonHang"] = new SelectList(_context.DonHangs, "MaDonHang", "MaDonHang", donHang.MaDonHang);
   
            ViewBag.Currentid = id;
            ViewBag.CurrentPage = page;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;
            ViewBag.CurrentMaID = MaID;

            return View( donHang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> ChangeStatus(string id, int? page, string? MaID, string? search, double? minPrice, double? maxPrice, [Bind("MaDonHang,TongTien,TrangThaiDonHang,MaKhachHang,ThanhToan,VanChuyen,PhiVanChuyen,NgayDatHang")] DonHang donHang)
        {
            if (id != donHang.MaDonHang)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy chi tiết đơn hàng và include MaDonHangNavigation và MaSanPhamNavigation
                    // Lấy chi tiết đơn hàng và include MaDonHangNavigation và MaSanPhamNavigation
                    var ctDonHang = await _context.DonHangs
                        .Include(dh => dh.ChiTietDonHangs)
                        .ThenInclude(c => c.MaSanPhamNavigation)
                        .Include(x => x.MaKhachHangNavigation)
                        .FirstOrDefaultAsync(m => m.MaDonHang == id);

                    if (ctDonHang != null)
                    {
                        // Sử dụng int.TryParse để chuyển đổi chuỗi thành số nguyên
                        if (int.TryParse(Request.Form["txtMaDM"], out int trangThai))
                        {
                            // Cập nhật trạng thái đơn hàng
                            ctDonHang.TrangThaiDonHang = trangThai;
                            if (ctDonHang.TrangThaiDonHang == 4) // Giả sử 4 đại diện cho "Hủy đơn hàng"
                            {
                                // Lặp qua từng mục trong đơn hàng và cập nhật tồn kho
                                foreach (var chiTietDonHang in ctDonHang.ChiTietDonHangs)
                                {
                                    // Lấy sản phẩm tương ứng
                                    var sanPham = chiTietDonHang.MaSanPhamNavigation;

                                    // Cập nhật số lượng sản phẩm trong kho
                                    sanPham.Slkho += chiTietDonHang.SoLuong;

                                    // Cập nhật sản phẩm trong ngữ cảnh
                                    _context.Update(sanPham);
                                }

                                // Lưu các thay đổi vào cơ sở dữ liệu
                                await _context.SaveChangesAsync();
                            }
                        }

                        _context.Update(ctDonHang);
                        await _context.SaveChangesAsync();
                        _notifyService.Success("Cập nhật trạng thái thành công");
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DonHangExists(donHang.MaDonHang))
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

            // Trong trường hợp có lỗi, bạn có thể in các lỗi ModelState để kiểm tra
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine($"Error: {error.ErrorMessage}");
                }
            }

            // Nếu có lỗi, hiển thị trang cập nhật với thông báo lỗi
            List<SelectListItem> trangThaiList = new List<SelectListItem>
    {
         new SelectListItem { Text = "Chờ xác nhận", Value = "1" },
        new SelectListItem { Text = "Đang vận chuyển", Value = "2" },
        new SelectListItem { Text = "Đã giao hàng", Value = "3" },
        new SelectListItem { Text = "Hủy đơn hàng", Value = "4" }
        // Thêm các trạng thái khác nếu cần
    };

            ViewData["TrangThai"] = new SelectList(trangThaiList, "Value", "Text", ModelState["MaDonHangNavigation.TrangThaiDonHang"].AttemptedValue);
            ViewData["MaDonHang"] = new SelectList(_context.DonHangs, "MaDonHang", "MaDonHang", donHang.MaDonHang);
            ViewBag.Currentid = id;
            ViewBag.CurrentPage = page;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;
            ViewBag.CurrentMaID = MaID;

            return View(donHang);
        }

        // GET: Admin/DonHangs/Edit/5
        public async Task<IActionResult> Edit(string id, int? page, string? MaID, string? search, double? minPrice, double? maxPrice)
        {
            if (id == null || _context.DonHangs == null)
            {
                return NotFound();
            }

            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang == null)
            {
                return NotFound();
            }
            ViewData["MaKhachHang"] = new SelectList(_context.KhachHangs, "MaKhachHang", "MaKhachHang", donHang.MaKhachHang);
            return View(donHang);
        }

        // POST: Admin/DonHangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, int? page, string? MaID, string? search, double? minPrice, double? maxPrice, [Bind("MaDonHang,TongTien,TrangThaiDonHang,MaKhachHang,ThanhToan,VanChuyen,PhiVanChuyen,NgayDatHang")] DonHang donHang)
        {
            if (id != donHang.MaDonHang)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(donHang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DonHangExists(donHang.MaDonHang))
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
            ViewData["MaKhachHang"] = new SelectList(_context.KhachHangs, "MaKhachHang", "MaKhachHang", donHang.MaKhachHang);
            return View(donHang);
        }

        // GET: Admin/DonHangs/Delete/5
        public async Task<IActionResult> Delete(string id, int? page, string? MaID, string? search, double? minPrice, double? maxPrice)
        {
            if (id == null || _context.DonHangs == null)
            {
                return NotFound();
            }

            var donHang = await _context.DonHangs
                .Include(d => d.MaKhachHangNavigation)
                .FirstOrDefaultAsync(m => m.MaDonHang == id);
            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }

        // POST: Admin/DonHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id, int? page, string? MaID, string? search, double? minPrice, double? maxPrice)
        {
            if (_context.DonHangs == null)
            {
                return Problem("Entity set '_2025MyPhamContext.DonHangs'  is null.");
            }
            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang != null)
            {
                _context.DonHangs.Remove(donHang);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { page = page, MaID = MaID, search = search, minPrice = minPrice, maxPrice = maxPrice });
        }

        private bool DonHangExists(string id)
        {
          return (_context.DonHangs?.Any(e => e.MaDonHang == id)).GetValueOrDefault();
        }
    }
}
