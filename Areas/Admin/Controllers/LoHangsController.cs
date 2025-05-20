using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
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
    public class LoHangsController : Controller
    {
        private readonly _2025MyPhamContext _context;

        public INotyfService _notifyService { get; }

        public LoHangsController(_2025MyPhamContext context, INotyfService notifyService)
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
                  .Where(lh => lh.TrangThaiLH == 1);



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

        public async Task<IActionResult> DetailsConfirmed(string id, int? page, string? MaID, string? search, double? minPrice, double? maxPrice)
        {
            if (_context.LoHangs == null)
            {
                return Problem("Entity set '_2025MyPhamContext.LoHangs' is null.");
            }
            // Sau khi xóa thành công, bạn có thể chuyển hướng trở lại trang chứa sản phẩm vừa xóa bằng cách truyền tham số `page`.
            // Nếu `page` không có giá trị, bạn có thể mặc định nó về một trang cụ thể (ví dụ: 1).
            return RedirectToAction("Index", new { page = page, MaID = MaID, search = search, minPrice = minPrice, maxPrice = maxPrice });

        }
        // GET: Admin/LoHangs/Create
        public IActionResult NhapKho(string id, float? Gia)
        {
            ViewBag.CurrentGia = Gia;
            ViewBag.CurrentMaSP = id;
            ViewData["MaNhaPp"] = new SelectList(_context.NhaPhanPhois, "MaNhaPp", "TenNhaPp");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> NhapKho(Kho nhapKho, string id, float? Gia)
        {
            ViewBag.CurrentGia = Gia;
            ViewBag.CurrentMaSP = id;
            ViewData["MaNhaPp"] = new SelectList(_context.NhaPhanPhois, "MaNhaPp", "TenNhaPp");
            var loHangExists = await _context.LoHangs.AnyAsync(lh => lh.MaLoHang == nhapKho.MaLoHang);
            if (loHangExists)
            {
                ModelState.AddModelError("MaLoHang", "Mã lô hàng đã tồn tại.");
                return View(nhapKho); // Trả về view với lỗi hiển thị
            }

            if (string.IsNullOrEmpty(nhapKho.MaLoHang))
            {
                ModelState.AddModelError("MaLoHang", "Mã lô hàng là trường bắt buộc.");
            }

            if (!Regex.IsMatch(nhapKho.MaLoHang, @"^LH\d{3,}$"))
            {
                // Nếu mã hãng không đúng định dạng, thêm lỗi vào ModelState
                ModelState.AddModelError("MaLoHang", "Mã lô hàng phải có dạng 'L' + 3 số tự nhiên bất kỳ.");
            }

            if (nhapKho.SoLuong == null)
            {
                ModelState.AddModelError("SoLuong", "Số lượng không được để trống!");
                return View(nhapKho); // Trả về view với lỗi hiển thị
            }

            if (nhapKho.GiaLo == null)
            {
                ModelState.AddModelError("GiaLo", "Giá lô không được để trống!");
                return View(nhapKho); // Trả về view với lỗi hiển thị
            }

            if (nhapKho.HSD == null)
            {
                ModelState.AddModelError("HSD", "Hạn sử dụng không được để trống!");
                return View(nhapKho); // Trả về view với lỗi hiển thị
            }


            string salt = Utilities.GetRandomKey();
            LoHang khachhang = new LoHang
            {
                MaLoHang = nhapKho.MaLoHang,
                MaNhaPp = "1",
                NgayNhan = DateTime.Now,
                GiaLo = nhapKho.GiaLo * nhapKho.SoLuong,
                HSD = nhapKho.HSD,
                TrangThaiLH = 1

            };
            _context.Add(khachhang);
            await _context.SaveChangesAsync();
            ChiTietLoHang loHang = new ChiTietLoHang
            {
                MaSanPham = nhapKho.TenSanPham,
                MaLoHang = nhapKho.MaLoHang,
                SoLuong = nhapKho.SoLuong,
                GiaNhap = nhapKho.GiaLo,
                DaBan = 0,
                HSDSP = nhapKho.HSD

            };

            _context.Add(loHang);
            await _context.SaveChangesAsync();
            var sanPham = _context.SanPhams.Find(nhapKho.TenSanPham);
            if (sanPham != null)
            {
                // Trừ số lượng đã mua từ số lượng tồn kho
                sanPham.Slkho += nhapKho.SoLuong;
                _context.Update(sanPham); // Sử dụng Update thay vì Add
                await _context.SaveChangesAsync();

            }

            return RedirectToAction("Index", "SanPhams");
        }
        public IActionResult Create()
        {
            ViewData["MaNhaPp"] = new SelectList(_context.NhaPhanPhois, "MaNhaPp", "TenNhaPp");
            return View();
        }

        // POST: Admin/LoHangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaLoHang,NgayNhan,MaNhaPp,GiaLo")] LoHang loHang)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loHang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaNhaPp"] = new SelectList(_context.NhaPhanPhois, "MaNhaPp", "MaNhaPp", loHang.MaNhaPp);
            return View(loHang);
        }

        // GET: Admin/LoHangs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.LoHangs == null)
            {
                return NotFound();
            }

            var loHang = await _context.LoHangs.FindAsync(id);
            if (loHang == null)
            {
                return NotFound();
            }
            ViewData["MaNhaPp"] = new SelectList(_context.NhaPhanPhois, "MaNhaPp", "MaNhaPp", loHang.MaNhaPp);
            return View(loHang);
        }

        // POST: Admin/LoHangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaLoHang,NgayNhan,MaNhaPp,GiaLo")] LoHang loHang)
        {
            if (id != loHang.MaLoHang)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loHang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoHangExists(loHang.MaLoHang))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaNhaPp"] = new SelectList(_context.NhaPhanPhois, "MaNhaPp", "MaNhaPp", loHang.MaNhaPp);
            return View(loHang);
        }

        // GET: Admin/LoHangs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.LoHangs == null)
            {
                return NotFound();
            }

            var loHang = await _context.LoHangs
                .Include(l => l.MaNhaPpNavigation)
                .FirstOrDefaultAsync(m => m.MaLoHang == id);
            if (loHang == null)
            {
                return NotFound();
            }

            return View(loHang);
        }

        // POST: Admin/LoHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.LoHangs == null)
            {
                return Problem("Entity set '_2025MyPhamContext.LoHangs'  is null.");
            }
            var loHang = await _context.LoHangs.FindAsync(id);
            if (loHang != null)
            {
                _context.LoHangs.Remove(loHang);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoHangExists(string id)
        {
            return (_context.LoHangs?.Any(e => e.MaLoHang == id)).GetValueOrDefault();
        }


        public async Task<IActionResult> XuLy(string id, Kho nhapKho)
        {
            var loHangToUpdate = await _context.LoHangs
                .Include(dh => dh.ChiTietLoHangs).ThenInclude(c => c.MaSanPhamNavigation)
                
                .FirstOrDefaultAsync(lh => lh.MaLoHang == id);
            
            if (loHangToUpdate != null)
            {
                loHangToUpdate.TrangThaiLH = 2;
                _context.Update(loHangToUpdate); // Sử dụng Update thay vì Add
                await _context.SaveChangesAsync();

            }

            var chiTietLH =  _context.ChiTietLoHangs.Where(ct => ct.MaLoHang == loHangToUpdate.MaLoHang && ct.HSDSP == loHangToUpdate.HSD).FirstOrDefault();

            

            var sanPham = _context.SanPhams.Where( sp => sp.MaSanPham == chiTietLH.MaSanPham).FirstOrDefault();
            if (sanPham != null)
            {
                sanPham.Slkho = sanPham.Slkho - (chiTietLH.SoLuong - chiTietLH.DaBan);
                _context.Update(sanPham); // Sử dụng Update thay vì Add
                await _context.SaveChangesAsync();

            }

            return RedirectToAction("Index", "LoHangs");
        }

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