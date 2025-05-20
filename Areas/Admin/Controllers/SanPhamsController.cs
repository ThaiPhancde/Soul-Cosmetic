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
    [Authorize(Roles = "Employee,Admin")]
    public class SanPhamsController : Controller
    {
        private readonly _2025MyPhamContext _context;
        public INotyfService _notifyService { get; }

        public SanPhamsController(_2025MyPhamContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/SanPhams
        public IActionResult Index(int? page, string MaDM = "", string? MaID = "", string search = "", double? minPrice = null, double? maxPrice = null)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 5;

            IQueryable<SanPham> query = _context.SanPhams
                .AsNoTracking()
                .Include(x => x.MaDanhMucNavigation);

            if (!string.IsNullOrEmpty(MaDM))
            {
                query = query.Where(x => x.MaDanhMuc == MaDM);
            }
            if (!string.IsNullOrEmpty(MaID))
            {
                query = query.Where(x => x.MaSanPham.Contains(MaID));
            }


            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.TenSanPham.Contains(search));
            }

            if (minPrice != null)
            {
                query = query.Where(x => x.Gia >= minPrice);
            }

            if (maxPrice != null)
            {
                query = query.Where(x => x.Gia <= maxPrice);
            }

            var lsProducts = query.OrderByDescending(x => x.MaSanPham).ToList();

            PagedList<SanPham> models = new PagedList<SanPham>(lsProducts.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentMaDM = MaDM;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentMaID = MaID;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;  // Thiếu dấu "." ở đây.
            ViewData["DanhMuc"] = new SelectList(_context.DanhMucSanPhams, "MaDanhMuc", "TenDanhMuc", MaDM);

            return View(models);
        }



        public IActionResult Filtter(string MaDM, string search, double? minPrice, double? maxPrice, string? MaID)
        {
            var url = "/Admin/SanPhams?";
            if (MaID != null)
            {
                url += $"MaID={MaID}&";
            }
            if (!string.IsNullOrEmpty(MaDM))
            {
                url += $"MaDM={MaDM}&";
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





        // GET: Admin/SanPhams/Details/5
        public async Task<IActionResult> Details(string id, int? page, string? MaID, string? search, string? MaDM, double? minPrice, double? maxPrice)
        {
            if (id == null || _context.SanPhams == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams
                .Include(s => s.MaDanhMucNavigation)
                .FirstOrDefaultAsync(m => m.MaSanPham == id);
            if (sanPham == null)
            {
                return NotFound();
            }
          ViewBag.CurrentPage = page;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentMaDM = MaDM;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;
            ViewBag.CurrentMaID = MaID;
            return View(sanPham);
        }
        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DetailsConfirmed(string id, int? page, string? MaID, string? search, string? MaDM, double? minPrice, double? maxPrice)
        {
            if (_context.SanPhams == null)
            {
                return Problem("Entity set '_2025MyPhamContext.SanPhams' is null.");
            }
            // Sau khi xóa thành công, bạn có thể chuyển hướng trở lại trang chứa sản phẩm vừa xóa bằng cách truyền tham số `page`.
            // Nếu `page` không có giá trị, bạn có thể mặc định nó về một trang cụ thể (ví dụ: 1).
            return RedirectToAction("Index", new { page = page, MaDM = MaDM, MaID = MaID, search = search, minPrice = minPrice, maxPrice = maxPrice });

        }

        // GET: Admin/SanPhams/Create
        public IActionResult Create(int? page, string? search, string? MaDM, string? MaID, double? minPrice, double? maxPrice)
        {
            ViewData["DanhMuc"] = new SelectList(_context.DanhMucSanPhams, "MaDanhMuc", "TenDanhMuc");
            ViewBag.CurrentPage = page;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentMaDM = MaDM;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;
            ViewBag.CurrentMaID = MaID;
            return View();
        }

        // POST: Admin/SanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? page, [Bind("MaSanPham,TenSanPham,Mau,Anh,Gia,Slkho,GiaNhap,NgaySx,LuotMua,MaDanhMuc,HomeFlag,BestSellers,MoTa")] SanPham sanPham, IFormFile? fAnh)

        {
            if (string.IsNullOrEmpty(sanPham.MaSanPham))
            {
                ModelState.AddModelError("MaSanPham", "Mã sản phẩm là trường bắt buộc.");
            }

            if (string.IsNullOrEmpty(sanPham.TenSanPham))
            {
                ModelState.AddModelError("TenSanPham", "Xin hãy nhập tên của sản phẩm.");
            }

            if (sanPham.Gia < 0)
            {
                ModelState.AddModelError("Gia", "Giá bán phải lớn hơn hoặc bằng 0");
            }
            if (sanPham.Gia == null)
            {
                ModelState.AddModelError("Gia", "Xin hãy nhập giá bán của sản phẩm.");
            }

            //if (sanPham.Slkho == null)
            //{
            //    ModelState.AddModelError("Slkho", "Xin hãy nhập số lượng tồn kho.");
            //}
            //if (sanPham.Slkho < 0)
            //{
            //    ModelState.AddModelError("Slkho", "Số lượng phải lớn hơn hoặc bằng 0");
            //}
            if (sanPham.GiaNhap == null)
            {
                ModelState.AddModelError("GiaNhap", "Xin hãy điền giá nhập của sản phẩm.");
            }
            if (sanPham.GiaNhap < 0)
            {
                ModelState.AddModelError("GiaNhap", "Giá nhập hàng phải lớn hơn hoặc bằng 0");
            }
            if (sanPham.MaDanhMuc == null)
            {
                ModelState.AddModelError("MaDanhMuc", "Xin hãy chọn danh mục sản phẩm.");
            }
            if (sanPham.MoTa == null)
            {
                ModelState.AddModelError("MoTa", "Xin hãy điền mô tả sản phẩm.");
            }
            if (sanPham.Mau == null)
            {
                ModelState.AddModelError("Mau", "Xin hãy điền màu  sản phẩm.");
            }
            if (ModelState.IsValid)
            {
                if (_context.SanPhams.Any(x => x.MaSanPham == sanPham.MaSanPham))
                {
                    ModelState.AddModelError("MaSanPham", "Mã sản phẩm đã tồn tại. Vui lòng chọn mã khác.");
                    ViewData["DanhMuc"] = new SelectList(_context.DanhMucSanPhams, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
                    return View(sanPham);
                }

                sanPham.TenSanPham = Utilities.ToTitleCase(sanPham.TenSanPham);
                if (fAnh != null)
                {
                    string extention = Path.GetExtension(fAnh.FileName);
                    string image = Utilities.SEOUrl(sanPham.TenSanPham) + extention;
                    sanPham.Anh = await Utilities.UploadFile(fAnh, @"sanPhams", image.ToLower());
                }
                //if (string.IsNullOrEmpty(sanPham.Anh)) sanPham.Anh = "default.jpg";

                //sanPham.NgaySx = DateTime.Now;
                sanPham.Slkho = 0;
                sanPham.NgaySx = DateTime.Now;
                sanPham.BestSellers = false;
                sanPham.HomeFlag = false;
                sanPham.LuotMua = 0;
                sanPham.Slkho = 0;

                _context.Add(sanPham);
                await _context.SaveChangesAsync();
                _notifyService.Success("Tạo mới sản phẩm thành công");
                return RedirectToAction("Index", new {page = page});
            }
            ViewData["DanhMuc"] = new SelectList(_context.DanhMucSanPhams, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            return View(sanPham);
        }

        public IActionResult Them(int? page, string? search, string? MaDM, string? MaID, double? minPrice, double? maxPrice)
        {
            ViewData["DanhMuc"] = new SelectList(_context.DanhMucSanPhams, "MaDanhMuc", "TenDanhMuc");
            ViewBag.CurrentPage = page;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentMaDM = MaDM;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;
            ViewBag.CurrentMaID = MaID;
            return View();
        }

        // POST: Admin/SanPhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Them(int? page, [Bind("MaSanPham,TenSanPham,Mau,Anh,Gia,KhuyenMai,Slkho,GiaNhap,NgaySx,LuotMua,MaDanhMuc,HomeFlag,BestSellers,MoTa,Active")] SanPham sanPham, IFormFile? fAnh)

        {
            if (string.IsNullOrEmpty(sanPham.MaSanPham))
            {
                ModelState.AddModelError("MaSanPham", "Mã sản phẩm là trường bắt buộc.");
            }

            if (string.IsNullOrEmpty(sanPham.TenSanPham))
            {
                ModelState.AddModelError("TenSanPham", "Xin hãy nhập tên của sản phẩm.");
            }

            if (sanPham.Gia < 0)
            {
                ModelState.AddModelError("Gia", "Giá bán phải lớn hơn hoặc bằng 0");
            }
            if (sanPham.Gia == null)
            {
                ModelState.AddModelError("Gia", "Xin hãy nhập giá bán của sản phẩm.");
            }

            if (sanPham.Slkho == null)
            {
                ModelState.AddModelError("Slkho", "Xin hãy nhập số lượng tồn kho.");
            }
            if (sanPham.Slkho < 0)
            {
                ModelState.AddModelError("Slkho", "Số lượng phải lớn hơn hoặc bằng 0");
            }
            if (sanPham.GiaNhap == null)
            {
                ModelState.AddModelError("GiaNhap", "Xin hãy điền giá nhập của sản phẩm.");
            }
            if (sanPham.GiaNhap < 0)
            {
                ModelState.AddModelError("GiaNhap", "Giá nhập hàng phải lớn hơn hoặc bằng 0");
            }
            if (sanPham.MaDanhMuc == null)
            {
                ModelState.AddModelError("MaDanhMuc", "Xin hãy chọn danh mục sản phẩm.");
            }
            if (sanPham.MoTa == null)
            {
                ModelState.AddModelError("MoTa", "Xin hãy điền mô tả sản phẩm.");
            }
            if (sanPham.Mau == null)
            {
                ModelState.AddModelError("Mau", "Xin hãy điền màu  sản phẩm.");
            }
            if (ModelState.IsValid)
            {
                if (_context.SanPhams.Any(x => x.MaSanPham == sanPham.MaSanPham))
                {
                    ModelState.AddModelError("MaSanPham", "Mã sản phẩm đã tồn tại. Vui lòng chọn mã khác.");
                    ViewData["DanhMuc"] = new SelectList(_context.DanhMucSanPhams, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
                    return View(sanPham);
                }

                sanPham.TenSanPham = Utilities.ToTitleCase(sanPham.TenSanPham);
                if (fAnh != null)
                {
                    string extention = Path.GetExtension(fAnh.FileName);
                    string image = Utilities.SEOUrl(sanPham.TenSanPham) + extention;
                    sanPham.Anh = await Utilities.UploadFile(fAnh, @"sanPhams", image.ToLower());
                }
                //if (string.IsNullOrEmpty(sanPham.Anh)) sanPham.Anh = "default.jpg";

                //sanPham.NgaySx = DateTime.Now;
                sanPham.NgaySx = DateTime.Now;
                _context.Add(sanPham);
                await _context.SaveChangesAsync();
                _notifyService.Success("Tạo mới sản phẩm thành công");
                return RedirectToAction("Index", new { page = page });
            }
            ViewData["DanhMuc"] = new SelectList(_context.DanhMucSanPhams, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            return View(sanPham);
        }
        // GET: Admin/SanPhams/Edit/5
        public async Task<IActionResult> Edit(string id, string? MaID, string? search, string? MaDM, double? minPrice, double? maxPrice)
        {
            if (id == null || _context.SanPhams == null)
            {
                return NotFound();
            }

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                return NotFound();
            }
            ViewData["DanhMuc"] = new SelectList(_context.DanhMucSanPhams, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
   
           
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentMaDM = MaDM;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;
            ViewBag.CurrentMaID = MaID;

            return View(sanPham);
        }

        // POST: Admin/SanPhams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string? MaID, string? search, string? MaDM, double? minPrice, double? maxPrice, [Bind("MaSanPham,TenSanPham,Mau,Anh,Gia,KhuyenMai,Slkho,GiaNhap,NgaySx,LuotMua,MaDanhMuc,HomeFlag,BestSellers,MoTa,Active")] SanPham sanPham, IFormFile? fAnh)
        {
            if (id != sanPham.MaSanPham)
            {
                return NotFound();
            }
            if (string.IsNullOrEmpty(sanPham.MaSanPham))
            {
                ModelState.AddModelError("MaSanPham", "Mã sản phẩm là trường bắt buộc.");
            }

            if (string.IsNullOrEmpty(sanPham.TenSanPham))
            {
                ModelState.AddModelError("TenSanPham", "Xin hãy nhập tên của sản phẩm.");
            }

            if (sanPham.Gia < 0)
            {
                ModelState.AddModelError("Gia", "Giá bán không là số âm");
            }
            if (sanPham.Gia == null)
            {
                ModelState.AddModelError("Gia", "Xin hãy nhập giá bán của sản phẩm.");
            }

            if (sanPham.Slkho == null)
            {
                ModelState.AddModelError("Slkho", "Xin hãy nhập số lượng tồn kho.");
            }
            if (sanPham.Slkho < 0)
            {
                ModelState.AddModelError("Slkho", "Số lượng phải lớn hơn hoặc bằng 0");
            }
            if (sanPham.GiaNhap == null)
            {
                ModelState.AddModelError("GiaNhap", "Xin hãy điền giá nhập của sản phẩm.");
            }
            if (sanPham.GiaNhap < 0)
            {
                ModelState.AddModelError("GiaNhap", "Giá nhập hàng phải lớn hơn hoặc bằng 0");
            }
            if (sanPham.MaDanhMuc == null)
            {
                ModelState.AddModelError("MaDanhMuc", "Xin hãy chọn danh mục sản phẩm.");
            }
            if (sanPham.MoTa == null)
            {
                ModelState.AddModelError("MoTa", "Xin hãy điền mô tả sản phẩm.");
            }
            if (sanPham.Mau == null)
            {
                ModelState.AddModelError("Mau", "Xin hãy điền màu  sản phẩm.");
            }
            if (sanPham.Gia <= 1000)
            {
                ModelState.AddModelError("GiaBan", "Giá bán phải lớn hơn 1000");
            }
            if (sanPham.GiaNhap <= 1000)
            {
                ModelState.AddModelError("GiaNhap", "Giá nhập phải lớn hơn 1000");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    sanPham.TenSanPham = Utilities.ToTitleCase(sanPham.TenSanPham);

                    if (fAnh != null)
                    {
                        // Có tệp hình ảnh mới được chọn
                        string extention = Path.GetExtension(fAnh.FileName);
                        string image = Utilities.SEOUrl(sanPham.TenSanPham) + extention;
                        sanPham.Anh = await Utilities.UploadFile(fAnh, @"sanPhams", image.ToLower());
                    }

                    if (string.IsNullOrEmpty(sanPham.Anh))
                    {
                        // Nếu không có hình ảnh mới, giữ nguyên hình ảnh hiện tại
                        sanPham.Anh = "default.jpg";
                    }
                    sanPham.NgaySua = DateTime.Now;
                    sanPham.BestSellers = false;
                    sanPham.HomeFlag = false;
                    _context.Update(sanPham);
                    await _context.SaveChangesAsync();
                    _notifyService.Success("Cập nhật sản phẩm thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanPhamExists(sanPham.MaSanPham))
                    {
                        _notifyService.Success("Có lỗi xảy ra");
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", new { MaDM = MaDM, MaID = MaID, search = search, minPrice = minPrice, maxPrice = maxPrice });
            }

            ViewData["DanhMuc"] = new SelectList(_context.DanhMucSanPhams, "MaDanhMuc", "MaDanhMuc", sanPham.MaDanhMuc);
            return View(sanPham);
        }


        // GET: Admin/SanPhams/Delete/5
      
        public async Task<IActionResult> Delete(string id, int? page, string? MaID, string? search, string? MaDM, double? minPrice, double? maxPrice)
{
    if (id == null || _context.SanPhams == null)
    {
        return NotFound();
    }

    var sanPham = await _context.SanPhams
        .Include(s => s.MaDanhMucNavigation)
        .FirstOrDefaultAsync(m => m.MaSanPham == id);
    if (sanPham == null)
    {
        return NotFound();
    }

            ViewBag.CurrentPage = page;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentMaDM = MaDM; // Lưu trạng thái trang hiện tại
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;
            ViewBag.CurrentMaID = MaID;

            return View(sanPham);
}


        // POST: Admin/SanPhams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(string id, string? MaID, int? page, string? search, string? MaDM, double? minPrice, double? maxPrice)
        {
            if (_context.SanPhams == null || _context.DonHangs == null)
            {
                return Problem("Entity set '_2025MyPhamContext.SanPhams' or '_2025MyPhamContext.DonHangs' is null.");
            }

            // Lấy thông tin sản phẩm cùng với danh sách đơn hàng liên quan
            var sanPham = await _context.SanPhams
                .Include(sp => sp.ChiTietDonHangs)
                .ThenInclude(sp => sp.MaDonHangNavigation)// Nạp danh sách đơn hàng liên quan
                .Include(sp=>sp.ChiTietLoHangs)
                .FirstOrDefaultAsync(sp => sp.MaSanPham == id);

            if (sanPham != null)
            {
                // Sử dụng RemoveRange để xóa danh sách đơn hàng liên quan
               
                _context.ChiTietDonHangs.RemoveRange(sanPham.ChiTietDonHangs);
                _context.ChiTietLoHangs.RemoveRange(sanPham.ChiTietLoHangs);
                // Xóa sản phẩm
                _context.SanPhams.Remove(sanPham);

                await _context.SaveChangesAsync();
                _notifyService.Success("Xóa sản phẩm thành công");
            }

            // Chuyển hướng trở lại trang chứa sản phẩm vừa xóa
            return RedirectToAction("Index", new { page = page, MaDM = MaDM, MaID = MaID, search = search, minPrice = minPrice, maxPrice = maxPrice });
        }



        private bool SanPhamExists(string id)
        {
          return (_context.SanPhams?.Any(e => e.MaSanPham == id)).GetValueOrDefault();
        }



    }
}
