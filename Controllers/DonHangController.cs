using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using MyPhamSoul.Models;
using PagedList.Core;
using MyPhamSoul.Services;

namespace MyPhamCpuheilinus.Controllers
{
    public class DonHangController : Controller
    {
        private readonly _2025MyPhamContext db;
        public INotyfService _notifyService { get; }
        private readonly IShippingService _shippingService;

        public DonHangController(_2025MyPhamContext _db, INotyfService notifyService, IShippingService shippingService)
        {
            db = _db;
            _notifyService = notifyService;
            _shippingService = shippingService;
        }

        public IActionResult Index(int? page, string? MaID = null, string search = "", double? minPrice = null, double? maxPrice = null)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;
            var taikhoanID = HttpContext.Session.GetString("AccountId");
            if (taikhoanID != null)
            {
                int accountId = int.Parse(taikhoanID);
                IQueryable<DonHang> query = db.DonHangs
                .AsNoTracking()
                .Include(dh => dh.ChiTietDonHangs).ThenInclude(c => c.MaSanPhamNavigation)
                .Include(x => x.MaKhachHangNavigation);

                // Lọc theo AccountId của khách hàng đã đăng nhập
                query = query.Where(x => x.MaKhachHangNavigation.AccountId == accountId);
                if (!string.IsNullOrEmpty(MaID))
                {
                    query = query.Where(x => x.MaDonHang.Contains(MaID));
                }

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(x => x.MaKhachHangNavigation.TenKhachHang.Contains(search));
                }

                if (minPrice != null)
                {
                    query = query.Where(x => x.TongTien >= minPrice);
                }

                if (maxPrice != null)
                {
                    query = query.Where(x => x.TongTien <= maxPrice);
                }

                var lsDonHangs = query.OrderByDescending(x => x.NgayDatHang).ToList();

                PagedList<DonHang> models = new PagedList<DonHang>(lsDonHangs.AsQueryable(), pageNumber, pageSize);

                ViewBag.CurrentMaID = MaID;
                ViewBag.CurrentPage = pageNumber;
                ViewBag.CurrentSearch = search;
                ViewBag.CurrentMinPrice = minPrice;
                ViewBag.CurrentMaxPrice = maxPrice;

                return View(models);
            }
            return RedirectToAction("Login", "Accounts");
        }

        public IActionResult ChiTietDonHang()
        {
            var donHangList = db.DonHangs
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
            var donHang = db.ChiTietDonHangs.Find(donHangId);
            if (donHang == null)
            {
                return NotFound();
            }

            donHang.MaDonHangNavigation.TrangThaiDonHang = trangThaiDonHang;
            db.SaveChanges();

            return Ok();
        }

        public IActionResult Filtter(string search, double? minPrice, double? maxPrice, string? MaID)
        {
            var url = "/DonHang?";
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

        // GET: Admin/DonHangs/Details/5
        public async Task<IActionResult> Details(string id, int? page, string? MaID, string? search, double? minPrice, double? maxPrice)
        {
            if (id == null || db.DonHangs == null)
            {
                return NotFound();
            }

            var donHangs = await db.DonHangs
               .AsNoTracking()
               .Include(dh => dh.ChiTietDonHangs)
               .ThenInclude(c => c.MaSanPhamNavigation)
                .Include(x => x.MaKhachHangNavigation)
                .FirstOrDefaultAsync(m => m.MaDonHang == id);
            if (donHangs == null)
            {
                return NotFound();
            }
            var donhang = db.DonHangs
                .AsNoTracking()
                .Include(dh => dh.ChiTietDonHangs)
                .ThenInclude(c => c.MaSanPhamNavigation)
                .Include(x => x.MaKhachHangNavigation)
                .AsNoTracking().Where(x => x.MaDonHang == donHangs.MaDonHang)
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
            if (db.DonHangs == null)
            {
                return Problem("Entity set '_2025MyPhamContext.DonHangs' is null.");
            }
            // Sau khi xóa thành công, bạn có thể chuyển hướng trở lại trang chứa sản phẩm vừa xóa bằng cách truyền tham số `page`.
            // Nếu `page` không có giá trị, bạn có thể mặc định nó về một trang cụ thể (ví dụ: 1).
            return RedirectToAction("Index", new { page = page, MaID = MaID, search = search, minPrice = minPrice, maxPrice = maxPrice });

        }
        public async Task<IActionResult> ChangeStatus(string id, int? page, string? MaID, string? search, double? minPrice, double? maxPrice)
        {
            if (id == null || db.DonHangs == null)
            {
                return NotFound();
            }

            var donHang = await db.DonHangs
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
            ViewData["MaDonHang"] = new SelectList(db.DonHangs, "MaDonHang", "MaDonHang", donHang.MaDonHang);

            ViewBag.Currentid = id;
            ViewBag.CurrentPage = page;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;
            ViewBag.CurrentMaID = MaID;

            return View(donHang);
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
                    var ctDonHang = await db.DonHangs
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
                                    db.Update(sanPham);
                                }

                                // Lưu các thay đổi vào cơ sở dữ liệu
                                await db.SaveChangesAsync();
                            }
                            if (ctDonHang.TrangThaiDonHang == 2)
                            {
                                var shipment = db.Shipments.FirstOrDefault(s => s.OrderId == ctDonHang.MaDonHang);
                                if (shipment == null)
                                {
                                    try
                                    {
                                        var req = new ShipmentRequest
                                        {
                                            OrderCode = ctDonHang.MaDonHang,
                                            FromDistrictId = 1450,
                                            ToDistrictId = 1450,
                                            ToWardCode = "10000", // Mã phường xã của người nhận
                                            ToName = ctDonHang.MaKhachHangNavigation.TenKhachHang,
                                            ToPhone = ctDonHang.MaKhachHangNavigation.SoDienThoai,
                                            ToAddress = ctDonHang.MaKhachHangNavigation.DiaChi,
                                            Weight = ctDonHang.ChiTietDonHangs.Sum(x => x.SoLuong.GetValueOrDefault()) * 500,
                                            OrderValue = (decimal)ctDonHang.TongThanhToan.GetValueOrDefault()
                                        };
                                        var code = _shippingService.CreateShipmentAsync(req).Result;
                                        shipment = new Shipment
                                        {
                                            OrderId = ctDonHang.MaDonHang,
                                            ProviderId = 1,
                                            ProviderOrderCode = code,
                                            ShippingFee = (decimal)ctDonHang.PhiVanChuyen.GetValueOrDefault(),
                                            Status = ShipmentStatus.Confirmed.ToString()
                                        };
                                        db.Shipments.Add(shipment);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("GHN create order error " + ex.Message);
                                    }
                                }
                                else
                                {
                                    shipment.Status = ShipmentStatus.Confirmed.ToString();
                                    shipment.UpdatedAt = DateTime.UtcNow;
                                    db.Shipments.Update(shipment);
                                }
                            }
                        }

                        db.Update(ctDonHang);
                        await db.SaveChangesAsync();
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
            ViewData["MaDonHang"] = new SelectList(db.DonHangs, "MaDonHang", "MaDonHang", donHang.MaDonHang);
            ViewBag.Currentid = id;
            ViewBag.CurrentPage = page;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;
            ViewBag.CurrentMaID = MaID;

            return View(donHang);
        }
        private bool DonHangExists(string id)
        {
            return (db.DonHangs?.Any(e => e.MaDonHang == id)).GetValueOrDefault();
        }
    }
}