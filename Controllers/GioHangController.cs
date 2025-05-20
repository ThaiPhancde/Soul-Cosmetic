using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyPhamSoul.Controllers;
using MyPhamSoul.Helpper;
using MyPhamSoul.Infrastructure;
using MyPhamSoul.Models;
using MyPhamSoul.ModelViews;
using MyPhamSoul.Services;
using Org.BouncyCastle.Asn1.Pkcs;
using System.Data;
using System.Net;
using System.Web.Helpers;


namespace MyPhamCpuheilinus.Controllers
{
    public class GioHangController : Controller
    {
        private readonly _2025MyPhamContext _context;
        private readonly PaypalClient _paypalClient;
        private readonly IMailService _mailService;
        public GioHang? GioHang { get; set; }
        public KhachHang? KhachHang { get; set; }
        _2025MyPhamContext db = new _2025MyPhamContext();
        public INotyfService _notifyService { get; }
        private readonly ILogger<GioHangController> _logger;
        private readonly IVnPayService _vnPayservice;
        private readonly IShippingService _shippingService;

        public GioHangController(ILogger<GioHangController> logger, INotyfService notifyService, IVnPayService vnPayService, IMailService mailService, _2025MyPhamContext context, PaypalClient paypalClient, IShippingService shippingService)
        {
            _logger = logger;
            _notifyService = notifyService;
            _vnPayservice = vnPayService;
            _mailService = mailService;
            _context = context;
            _paypalClient = paypalClient;
            _shippingService = shippingService;
        }

        // Thêm phương thức mới để lấy thông tin tổng giỏ hàng qua AJAX
        [HttpGet]
        public IActionResult GetCartTotals()
        {
            GioHang = HttpContext.Session.GetJson<GioHang>("giohang") ?? new GioHang();
            
            // Tính tổng tiền
            var subtotal = GioHang.ComputeTotalValues();
            var total = subtotal + 10000; // Phí vận chuyển
            
            // Tạo danh sách thông tin các sản phẩm
            var items = GioHang.Lines.Select(line => new {
                maSanPham = line.SanPham.MaSanPham,
                soLuong = line.SoLuong,
                gia = line.SanPham.Gia,
                total = line.SanPham.Gia * line.SoLuong
            });
            
            return Json(new {
                subtotal = subtotal,
                total = total,
                items = items
            });
        }

        [Authorize(Roles = "Customer, Admin, Employee")]

        [HttpPost]
        public IActionResult AddGioHang1(string maSanPham)
        {
            SanPham? sanpham = db.SanPhams.FirstOrDefault(p => p.MaSanPham == maSanPham);
            if (sanpham.Slkho == 0)
            {
                _notifyService.Error("Sản phẩm đã hết hàng");
                return RedirectToAction("SanPhamTheoDanhMuc");
            }

            if (sanpham != null)
            {
                GioHang = HttpContext.Session.GetJson<GioHang>("giohang") ?? new GioHang();
                GioHang.AddItem(sanpham, 1);
                HttpContext.Session.SetJson("giohang", GioHang);
                
                // Nếu là yêu cầu AJAX, trả về JSON
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }
                
                TempData["ThongBao"] = "Sản phẩm đã được thêm vào giỏ hàng";
            }
            return View("GioHang", GioHang);
        }
        
        [HttpPost]
        public IActionResult AddGioHang(string maSanPham, int soLuong)
        {
            SanPham? sanpham = db.SanPhams.FirstOrDefault(p => p.MaSanPham == maSanPham);
            if (sanpham == null)
            {
                // Xử lý khi không tìm thấy sản phẩm
                return NotFound();
            }

            if (sanpham.Slkho == 0)
            {
                _notifyService.Error("Sản phẩm đã hết hàng");
                return RedirectToAction("SanPhamTheoDanhMuc");
            }

            if ( soLuong > sanpham.Slkho)
            {
                _notifyService.Error("Số lượng không lớn hơn số lượng trong kho");
                return RedirectToAction("SanPhamTheoDanhMuc");
            }

            GioHang = HttpContext.Session.GetJson<GioHang>("giohang") ?? new GioHang();
            GioHang.AddItem(sanpham, soLuong);
            HttpContext.Session.SetJson("giohang", GioHang);
            TempData["ThongBao"] = "Sản phẩm đã được thêm vào giỏ hàng";

            return View("GioHang", GioHang);
        }

        [HttpPost]
        public IActionResult UpdateQuantity(string maSanPham, int soLuong)
        {
            // Lấy giỏ hàng từ Session
            GioHang gioHang = HttpContext.Session.GetJson<GioHang>("giohang") ?? new GioHang();

            // Tìm dòng sản phẩm trong giỏ hàng
            GioHangLine line = gioHang.Lines.FirstOrDefault(l => l.SanPham.MaSanPham == maSanPham);

            if (line != null)
            {
                // Nếu số lượng mới nhập vào là 0, loại bỏ sản phẩm khỏi giỏ hàng
                if (soLuong == 0)
                {
                    gioHang.Lines.Remove(line);
                }
                else
                {
                    // Cập nhật số lượng sản phẩm trong giỏ hàng
                    gioHang.UpdateQuantity(maSanPham, soLuong);
                }

                // Lưu giỏ hàng đã được cập nhật vào Session
                HttpContext.Session.SetJson("giohang", gioHang);
                
                // Nếu là yêu cầu AJAX, trả về JSON
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }
            }

            // Trả về trang xem giỏ hàng
            return RedirectToAction("ViewGioHang", "GioHang");
        }


        [HttpPost]
        public IActionResult Remove_1_FromGioHang(string maSanPham)
        {
            GioHang gioHang = HttpContext.Session.GetJson<GioHang>("giohang") ?? new GioHang();

            // Tìm kiếm sản phẩm trong giỏ hàng
            GioHangLine lineToRemove = gioHang.Lines.FirstOrDefault(line => line.SanPham.MaSanPham == maSanPham);

            // Nếu sản phẩm được tìm thấy và số lượng lớn hơn 0, giảm số lượng đi 1
            if (lineToRemove != null && lineToRemove.SoLuong > 0)
            {
                gioHang.AddItem(lineToRemove.SanPham, -1);

                // Nếu số lượng sau khi giảm bằng 0, xóa sản phẩm khỏi giỏ hàng
                if (lineToRemove.SoLuong == 0)
                {
                    gioHang.Lines.Remove(lineToRemove);
                }

                var tongTien = gioHang.ComputeTotalValues();
                ViewBag.TongTien = tongTien;
                
                HttpContext.Session.SetJson("giohang", gioHang);
                
                // Nếu là yêu cầu AJAX, trả về JSON
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }
            }

            HttpContext.Session.SetJson("giohang", gioHang);

            return View("GioHang", gioHang);
        }

        public IActionResult RemoveFromGioHang(string maSanPham)
        {
            SanPham? sanpham = db.SanPhams.FirstOrDefault(p => p.MaSanPham == maSanPham);
            if (sanpham != null)
            {
                GioHang = HttpContext.Session.GetJson<GioHang>("giohang");
                GioHang.RemoveLine(sanpham);
                HttpContext.Session.SetJson("giohang", GioHang);
                
                // Nếu là yêu cầu AJAX, trả về JSON
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }
            }
            return View("GioHang", GioHang);
        }

        public async Task<IActionResult> CheckOut( int maKM)
        {
            ViewBag.PaypalClientdId = _paypalClient.ClientId;
            ViewBag.MaKM = maKM;
			// Lấy AccountId từ session
			var taikhoanID = HttpContext.Session.GetString("AccountId");
            if (string.IsNullOrEmpty(taikhoanID))
            {
                // Nếu chưa đăng nhập, có thể chuyển hướng người dùng đến trang đăng nhập
                return RedirectToAction("Login", "Accounts");
            }
            int id;
			int.TryParse(taikhoanID, out id);
			var khachHang = _context.Accounts.SingleOrDefault(kh => kh.AccountId == id);

			var khuyenMai = _context.KhuyenMais.SingleOrDefault(km => km.MaKM == maKM && km.AccountId == khachHang.AccountId);
            double TongGiamGia = 0;
            if(khuyenMai == null)
            {
                maKM = 0;
                TongGiamGia = 0;
            }
            else
            {
                TongGiamGia = khuyenMai.GiaGiam;
            }

			if (khuyenMai == null && maKM != 0)
			{
				ViewBag.ErrorMessage = "Mã giảm giá không hợp lệ.";
				// Hoặc có thể sử dụng TempData nếu muốn giữ thông báo qua một redirect
				// TempData["ErrorMessage"] = "Mã giảm giá không hợp lệ.";
			}

			// Kiểm tra đăng nhập
			

            if (int.TryParse(taikhoanID, out var accountId))
            {
                // Truy vấn danh sách KhachHang thuộc Account có AccountId tương ứng
                var danhSachKhachHang = db.KhachHangs
                    .Where(kh => kh.AccountId == accountId)
                    .ToList();

                // Lấy thông tin giỏ hàng từ session
                var gioHang = HttpContext.Session.GetJson<MyPhamSoul.Models.GioHang>("giohang") ?? new MyPhamSoul.Models.GioHang();

                
                if (khuyenMai != null)
                {
                    gioHang.TienGiam = khuyenMai.GiaGiam;
                    HttpContext.Session.SetInt32("MaKM", khuyenMai.MaKM);
                }

                HttpContext.Session.SetString("TongGiamGia", TongGiamGia.ToString());

                // Tính phí GHN demo (trường hợp lỗi sẽ fallback)
                try
                {
                    var fee = await _shippingService.CalculateFeeAsync(new ShippingRateRequest
                    {
                        FromDistrictId = 1450, // giả định kho của shop
                        ToDistrictId = 1450,
                        Weight = gioHang.TotalWeightGram(),
                        OrderValue = (decimal)gioHang.ComputeTotalValues()
                    });
                    gioHang.PhiVanChuyen = (double)fee;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Không lấy được phí GHN, dùng mặc định 10000");
                    gioHang.PhiVanChuyen = 10000;
                }

                // Kết hợp danh sách KhachHang và thông tin giỏ hàng
                var model = new Tuple<List<MyPhamSoul.Models.KhachHang>, MyPhamSoul.Models.GioHang>(danhSachKhachHang, gioHang);

                // Trả về View với model chứa cả danh sách KhachHang và thông tin giỏ hàng
                return View(model);
            }

            // Nếu có lỗi xử lý, có thể chuyển hướng người dùng đến trang lỗi hoặc trang mặc định
            return RedirectToAction("Login", "Accounts");
        }
        public IActionResult ViewGioHang()
        {
            var gioHang = HttpContext.Session.GetJson<GioHang>("giohang") ?? new GioHang();
            return View(gioHang);
        }
        private int GenerateUniqueCustomerCode()
        {
            Random random = new Random();
            int randomNumber = random.Next(10000, 99999); // Sinh ra số ngẫu nhiên từ 10,000 đến 99,999

            return randomNumber;
        }
        private string GenerateUniqueOrderCode()
        {
            Random random = new Random();
            string customerCode;

            // Lặp cho đến khi tạo ra một mã đơn hàng không trùng lặp trong CSDL
            do
            {
                int randomNumber = random.Next(10000, 99999); // Sinh ra số ngẫu nhiên từ 10,000 đến 99,999
                customerCode = "DH" + randomNumber.ToString();
            }
            while (OrderCodeExists(customerCode)); // Kiểm tra xem mã đơn hàng đã tồn tại trong CSDL chưa

            return customerCode;
        }

        private bool OrderCodeExists(string orderCode)
        {
            // Truy vấn CSDL để kiểm tra xem mã đơn hàng đã tồn tại trong CSDL chưa
            // Giả sử bạn sử dụng Entity Framework Core
            bool exists = db.DonHangs.Any(o => o.MaDonHang == orderCode);

            return exists;
        }

        private int GeneratePromoCode()
        {
            Random random = new Random();
            int promoCode = random.Next(1000, 9999); // Sinh ra số ngẫu nhiên từ 1000 đến 9999
            return promoCode;
        }

        private bool IsPromoCodeExists(int promoCode)
        {
            // Thực hiện truy vấn đến cơ sở dữ liệu để kiểm tra xem mã khuyến mãi đã tồn tại chưa
            var existingPromoCode = _context.KhuyenMais.FirstOrDefault(p => p.MaKM == promoCode);

            // Nếu mã khuyến mãi đã tồn tại trong CSDL, trả về true
            // Ngược lại, trả về false
            return existingPromoCode != null;
        }



        //   public ActionResult ThanhToan(string maKhachHang, string hoTen, string soDienThoai, string diaChi, string email)
        //   {
        //       var taikhoanID = HttpContext.Session.GetString("AccountId");
        //       var khachhang = new KhachHang
        //       {
        //           //MaKhachHang = GenerateUniqueCustomerCode(),
        //           TenKhachHang = hoTen,
        //           DiaChi = diaChi,
        //           SoDienThoai = soDienThoai,
        //           AccountId= Convert.ToInt32(taikhoanID)
        //       };
        //       db.KhachHangs.Add(khachhang);
        //       db.SaveChanges();

        //       var gioHang = HttpContext.Session.GetJson<GioHang>("giohang");
        //       var donHang = new DonHang
        //       {
        //           MaDonHang = GenerateUniqueOrderCode(),
        //           NgayDatHang = DateTime.Now,
        //           TongTien = gioHang.ComputeTotalValues(),
        //           TrangThaiDonHang = 1,
        //           MaKhachHang = khachhang.MaKhachHang,
        //           ThanhToan = true,
        //           VanChuyen = 1,
        //           PhiVanChuyen = 10000
        //       };
        //       db.DonHangs.Add(donHang);
        //       db.SaveChanges();
        //       foreach (var line in gioHang.Lines)
        //       {
        //           var chiTietDonHang = new ChiTietDonHang
        //           {
        //               MaDonHang = donHang.MaDonHang,
        //               MaSanPham = line.SanPham.MaSanPham,
        //               SoLuong = line.SoLuong,
        //               GiaBan = line.SanPham.Gia
        //           };
        //           db.ChiTietDonHangs.Add(chiTietDonHang);
        //           RemoveFromGioHang(line.SanPham.MaSanPham);
        //       }

        //       db.SaveChanges();

        //       foreach (var line in gioHang.Lines)
        //       {
        //           ;
        //           RemoveFromGioHang(line.SanPham.MaSanPham);
        //       }
        //;

        //       gioHang.Clear();


        //       return View("Success");
        //   }






        [HttpPost]
        public IActionResult SaveAddress(string hoTen, string diaChi, string soDienThoai, string email)
        {
            try
            {
                var taikhoanID = HttpContext.Session.GetString("AccountId");

                // Kiểm tra và chuyển đổi taikhoanID thành int
                if (!int.TryParse(taikhoanID, out int accountId))
                {
                    return Json(new { success = false, message = "AccountId không hợp lệ" });
                }

                // Tạo đối tượng KhachHang
                var khachhang = new KhachHang
                {
                    //MaKhachHang = GenerateUniqueCustomerCode(), // Cần triển khai hàm này
                    TenKhachHang = hoTen,
                    DiaChi = diaChi,
                    SoDienThoai = soDienThoai,
                    AccountId = accountId
                };

                // Lưu đối tượng vào cơ sở dữ liệu
                db.KhachHangs.Add(khachhang);
                db.SaveChanges();

                return RedirectToAction("CheckOut");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
        [HttpPost]
           public ActionResult ThanhToan(int selectedKhachHang, string payment = "COD")
   {
       try
       {
           HttpContext.Session.SetInt32("SelectedCustomerId", selectedKhachHang);

           double TongGiamGia = double.Parse(HttpContext.Session.GetString("TongGiamGia"));
           var maKM = HttpContext.Session.GetInt32("MaKM");

           var khuyenMai1 = _context.KhuyenMais.SingleOrDefault(km => km.MaKM == maKM);
           if (khuyenMai1 != null)
           {
               _context.KhuyenMais.Remove(khuyenMai1);
               _context.SaveChanges();
           }
           // Lấy thông tin của KhachHang đã chọn
           var selectedKhachHangObject = db.KhachHangs.Find(selectedKhachHang);

           if (selectedKhachHangObject != null)
           {
               // Thực hiện các bước thanh toán với selectedKhachHangObject
               var gioHang = HttpContext.Session.GetJson<GioHang>("giohang");
               var donHang = new DonHang
               {
                   MaDonHang = GenerateUniqueOrderCode(),
                   NgayDatHang = DateTime.Now,
                   TongTien = gioHang.ComputeTotalValues(),
                   TrangThaiDonHang = 1,
                   MaKhachHang = selectedKhachHangObject.MaKhachHang,
                   ThanhToan = true,
                   VanChuyen = 1,
                   PhiVanChuyen = 10000,
                   TienGiam = TongGiamGia,
                   TongThanhToan = gioHang.ComputeTotalValues() + 10000 - TongGiamGia,
                   PaymentMethod = "COD"  // Thanh toán tiền mặt
               };

               db.DonHangs.Add(donHang);
               db.SaveChanges();

               foreach (var line in gioHang.Lines)
               {
                   var sanPham = db.SanPhams.Find(line.SanPham.MaSanPham);

                   if (sanPham != null)
                   {
                       // Cập nhật số lượng tồn kho và lượt mua của sản phẩm
                       sanPham.Slkho -= line.SoLuong;
                       sanPham.LuotMua += line.SoLuong;
                       db.Update(sanPham);

                       // Tìm ChiTietLoHang có HSD gần nhất và MaSanPham khớp
                       var chiTietLoHangs = db.ChiTietLoHangs
                       .Where(ct => ct.MaSanPham == sanPham.MaSanPham && ct.HSDSP.Value.Date >= DateTime.Now)
                       .OrderBy(ct => ct.HSDSP)
                       .ToList();

                       int remainingQuantity = line.SoLuong;

                       foreach (var loHang in chiTietLoHangs)
                       {
                           int availableQuantity = (int)(loHang.SoLuong - loHang.DaBan);

                           if (availableQuantity >= remainingQuantity)
                           {
                               loHang.DaBan += remainingQuantity;
                               db.Update(loHang);
                               remainingQuantity = 0;
                               break;
                           }
                           else
                           {
                               loHang.DaBan = loHang.SoLuong; // bán hết lô hàng này
                               db.Update(loHang);
                               remainingQuantity -= availableQuantity;
                           }
                       }

                       db.SaveChanges();
                   }
                   db.SaveChanges();
               }

               // Lưu các thay đổi vào cơ sở dữ liệu
        


               var fullName = HttpContext.Session.GetString("AccountId");

               int accountId;
               int.TryParse(fullName, out accountId);

               var user = _context.Accounts.FirstOrDefault(a => a.AccountId == accountId);

               // Tạo khuyến mãi dựa trên giá trị đơn hàng
               if (gioHang.TongTienThanhToan > 500000 && gioHang.TongTienThanhToan < 1000000)
               {
                   var khuyenMai = new KhuyenMai
                   {
                       MaKM = GeneratePromoCode(),
                       TenKM = "Giảm giá 20.000 VNĐ cho đơn hàng",
                       GiaGiam = 20000,
                       NgayBD = DateTime.Now,
                       NgayKT = DateTime.Now.AddDays(30),
                       AccountId = user.AccountId


                   };
                   _context.KhuyenMais.Add(khuyenMai);
                   _context.SaveChanges();

                   var mailData2 = new MailData
                   {
                       ReceiverName = user.FullName, // Thay "Tên khách hàng" bằng tên thực sự của khách hàng
                       ReceiverEmail = user.AccountEmail, // Thay "email@example.com" bằng địa chỉ email thực sự của khách hàng
                       Title = "Thông báo khuyến mãi",
                       Body = "Chào " + user.FullName + ",\n\n"
   + "Chúng tôi rất vui thông báo đến bạn về mã khuyến mãi đặc biệt:\n\n"
   + "Mã khuyến mãi: " + khuyenMai.MaKM + "\n"
   + "Tên khuyến mãi: " + khuyenMai.TenKM + "\n"
   + "Giá giảm: " + khuyenMai.GiaGiam.ToString("N0") + "" + "VNĐ" + "\n"
   + "Thời gian bắt đầu: " + khuyenMai.NgayBD.ToString("dd/MM/yyyy") + "\n"
   + "Thời gian kết thúc: " + khuyenMai.NgayKT.ToString("dd/MM/yyyy") + "\n\n"
   + "Hãy sử dụng mã này để nhận ưu đãi đặc biệt khi mua hàng tiếp theo tại cửa hàng của chúng tôi.\n\n"
   + "Chúc bạn mua sắm vui vẻ và tiết kiệm!"
                   };
                   // Gửi email
                   _mailService.SendMail(mailData2);
               }


               if (gioHang.TongTienThanhToan > 1000000 && gioHang.TongTienThanhToan < 2000000)
               {
                   var khuyenMai = new KhuyenMai
                   {
                       MaKM = GeneratePromoCode(),
                       TenKM = "Giảm giá 50.000 VNĐ cho đơn hàng",
                       GiaGiam = 50000,
                       NgayBD = DateTime.Now,
                       NgayKT = DateTime.Now.AddDays(30),
                       AccountId = user.AccountId


                   };
                   _context.KhuyenMais.Add(khuyenMai);
                   _context.SaveChanges();

                   var mailData2 = new MailData
                   {
                       ReceiverName = user.FullName, // Thay "Tên khách hàng" bằng tên thực sự của khách hàng
                       ReceiverEmail = user.AccountEmail, // Thay "email@example.com" bằng địa chỉ email thực sự của khách hàng
                       Title = "Thông báo khuyến mãi",
                       Body = "Chào " + user.FullName + ",\n\n"
   + "Chúng tôi rất vui thông báo đến bạn về mã khuyến mãi đặc biệt:\n\n"
   + "Mã khuyến mãi: " + khuyenMai.MaKM + "\n"
   + "Tên khuyến mãi: " + khuyenMai.TenKM + "\n"
   + "Giá giảm: " + khuyenMai.GiaGiam.ToString("N0") + "" + "VNĐ" + "\n"
   + "Thời gian bắt đầu: " + khuyenMai.NgayBD.ToString("dd/MM/yyyy") + "\n"
   + "Thời gian kết thúc: " + khuyenMai.NgayKT.ToString("dd/MM/yyyy") + "\n\n"
   + "Hãy sử dụng mã này để nhận ưu đãi đặc biệt khi mua hàng tiếp theo tại cửa hàng của chúng tôi.\n\n"
   + "Chúc bạn mua sắm vui vẻ và tiết kiệm!"
                   };
                   // Gửi email
                   _mailService.SendMail(mailData2);
               }

               // Tạo chi tiết đơn hàng
               foreach (var line in gioHang.Lines)
               {
                   var chiTietDonHang = new ChiTietDonHang
                   {
                       MaDonHang = donHang.MaDonHang,
                       MaSanPham = line.SanPham.MaSanPham,
                       SoLuong = line.SoLuong,
                       GiaBan = line.SanPham.Gia
                   };
                   db.ChiTietDonHangs.Add(chiTietDonHang);
                   RemoveFromGioHang(line.SanPham.MaSanPham);
               }

               db.SaveChanges();

               // Gửi email xác nhận đơn hàng
               var mailData1 = new MailData
               {
                   ReceiverName = selectedKhachHangObject.TenKhachHang,
                   ReceiverEmail = user.AccountEmail,
                   Title = "Thông báo đặt hàng thành công",
                   Body = "Chào " + selectedKhachHangObject.TenKhachHang + ",\n\n"
       + "Đơn hàng của bạn đã được xác nhận thành công. Dưới đây là chi tiết đơn hàng:\n\n"
       + "Mã đơn hàng: " + donHang.MaDonHang + "\n"
       + "Ngày đặt hàng: " + DateTime.Now.ToString("dd/MM/yyyy") + "\n"
       + "Phương thức thanh toán: Thanh toán khi nhận hàng (COD)\n"
       + "Danh sách sản phẩm:\n"
               };

               foreach (var line in gioHang.Lines)
               {
                   mailData1.Body += "Tên sản phẩm: " + line.SanPham.TenSanPham + "\n"
                                    + "Số lượng: " + line.SoLuong + "\n"
                                    + "Giá bán: " + line.SanPham.Gia + "" + "VNĐ" + "\n";
                                    
               }

               var tienThanhToan = donHang.TongTien + donHang.PhiVanChuyen;

               mailData1.Body += "Tổng tiền: " + donHang.TongTien + "" + "VNĐ" + "\n"
                       + "Phí vận chuyển: " + donHang.PhiVanChuyen + "" + "VNĐ" + "\n";
                       
               if (TongGiamGia > 0)
               {
                   mailData1.Body += "Giảm giá: " + TongGiamGia + "" + "VNĐ" + "\n";
               }
               
               mailData1.Body += "Tiền thanh toán: " + donHang.TongThanhToan + "" + "VNĐ" + "\n"
                       + "Cảm ơn bạn đã mua hàng tại cửa hàng chúng tôi.";

               // Gửi email
               _mailService.SendMail(mailData1);

               // Xóa giỏ hàng sau khi đặt hàng thành công
               gioHang.Clear();
               HttpContext.Session.SetJson("giohang", gioHang);

               TempData["SuccessMessage"] = "Đã đặt hàng thành công.";

               // thêm tạo shipment GHN
               try
               {
                   var shipmentReq = new ShipmentRequest
                   {
                       OrderCode = donHang.MaDonHang,
                       FromDistrictId = 1450,
                       ToDistrictId = 1450,
                       ToWardCode = "10000", // Mã phường xã mặc định
                       ToName = selectedKhachHangObject.TenKhachHang,
                       ToPhone = selectedKhachHangObject.SoDienThoai,
                       ToAddress = selectedKhachHangObject.DiaChi,
                       Weight = gioHang.TotalWeightGram(),
                       OrderValue = (decimal)donHang.TongThanhToan!
                   };
                   var providerCode = _shippingService.CreateShipmentAsync(shipmentReq).Result;
                   var shipment = new Shipment
                   {
                       OrderId = donHang.MaDonHang,
                       ProviderId = 1,
                       ProviderOrderCode = providerCode,
                       ShippingFee = (decimal)donHang.PhiVanChuyen!,
                       Status = ShipmentStatus.Pending.ToString()
                   };
                   db.Shipments.Add(shipment);
                   db.SaveChanges();
               }
               catch (Exception ex)
               {
                   _logger.LogError(ex, "Lỗi tạo vận đơn GHN");
               }

               return View("Success");
           }
           else
           {
               TempData["ErrorMessage"] = "Không tìm thấy thông tin Khách Hàng đã chọn.";
           }
       }
       catch (Exception ex)
       {
           // Xử lý lỗi nếu có
           _logger.LogError($"Đã xảy ra lỗi trong quá trình thanh toán: {ex.Message}");
           TempData["ErrorMessage"] = $"Đã xảy ra lỗi trong quá trình thanh toán: {ex.Message}";
       }

       // Chuyển hướng về CheckOut hoặc trang khác tùy thuộc vào yêu cầu của bạn
       return RedirectToAction("CheckOut");
   }

        [Authorize]
        public IActionResult PaymentSuccess()
        {
            return View("Success");
        }

        [Authorize]
        public IActionResult PaymentFail()
        {
            return View();
        }

        [Authorize]
        public IActionResult PaymentCallBack()
        {
            try
            {
                Dictionary<string, string> vnpayData = new Dictionary<string, string>();
                
                // Lấy tất cả các tham số nhận được từ VNPAY
                _logger.LogInformation("Nhận được callback từ VNPay với các tham số:");
                foreach (var (key, value) in Request.Query)
                {
                    _logger.LogInformation($"{key}: {value}");
                    vnpayData.Add(key, value);
                }
                
                var response = _vnPayservice.PaymentExecute(Request.Query);
                
                // Nếu response là null, có lỗi trong quá trình xử lý
                if (response == null)
                {
                    _logger.LogError("Lỗi trong quá trình xử lý thanh toán VNPay: Response null");
                    TempData["Message"] = "Lỗi trong quá trình xử lý thanh toán VNPay";
                    return RedirectToAction("PaymentFail");
                }
                
                // Nếu không xác thực được chữ ký
                if (!response.Success)
                {
                    _logger.LogError($"Lỗi thanh toán VNPay: Xác thực không thành công, mã lỗi: {response.VnPayResponseCode}");
                    TempData["Message"] = $"Lỗi thanh toán VNPay: Xác thực chữ ký không thành công";
                    return RedirectToAction("PaymentFail");
                }
                
                // Xử lý mã lỗi từ VNPAY
                if (response.VnPayResponseCode != "00")
                {
                    string errorMessage = "Lỗi thanh toán VNPay";
                    // Xử lý mã lỗi cụ thể từ VNPay
                    switch (response.VnPayResponseCode)
                    {
                        case "01":
                            errorMessage = "Giao dịch đã tồn tại";
                            break;
                        case "02":
                            errorMessage = "Merchant không hợp lệ (kiểm tra lại vnp_TmnCode)";
                            break;
                        case "03":
                            errorMessage = "Dữ liệu gửi sang không đúng định dạng";
                            break;
                        case "04":
                            errorMessage = "Khởi tạo giao dịch không thành công";
                            break;
                        case "07":
                            errorMessage = "Giao dịch bị nghi ngờ gian lận";
                            break;
                        case "09":
                            errorMessage = "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng chưa đăng ký dịch vụ InternetBanking tại ngân hàng";
                            break;
                        case "10":
                            errorMessage = "Khách hàng xác thực thông tin thẻ/tài khoản không đúng quá 3 lần";
                            break;
                        case "11":
                            errorMessage = "Đã hết hạn chờ thanh toán";
                            break;
                        case "12":
                            errorMessage = "Thẻ/Tài khoản của khách hàng bị khóa";
                            break;
                        case "13":
                            errorMessage = "Khách hàng nhập sai mật khẩu thanh toán quá số lần quy định";
                            break;
                        case "24":
                            errorMessage = "Khách hàng hủy giao dịch";
                            break;
                        case "51":
                            errorMessage = "Tài khoản không đủ số dư để thực hiện giao dịch";
                            break;
                        case "65":
                            errorMessage = "Tài khoản của quý khách đã vượt quá hạn mức giao dịch trong ngày";
                            break;
                        case "75":
                            errorMessage = "Ngân hàng thanh toán đang bảo trì";
                            break;
                        case "79":
                            errorMessage = "Giao dịch không thành công do: Khách hàng không đúng thông tin xác thực";
                            break;
                        case "99":
                            errorMessage = "Lỗi không xác định";
                            break;
                        default:
                            errorMessage = $"Lỗi thanh toán VNPay: {response.VnPayResponseCode}";
                            break;
                    }
                    
                    _logger.LogError($"Lỗi thanh toán VNPay: {errorMessage}, mã lỗi: {response.VnPayResponseCode}");
                    TempData["Message"] = errorMessage;
                    return RedirectToAction("PaymentFail");
                }
                
                // Lấy thông tin đơn hàng từ session
                var orderCode = response.OrderId; // Sử dụng OrderId từ response
                if (string.IsNullOrEmpty(orderCode))
                {
                    orderCode = HttpContext.Session.GetString("VnPayOrderCode");
                    _logger.LogInformation($"Sử dụng mã đơn hàng từ session: {orderCode}");
                }
                
                if (string.IsNullOrEmpty(orderCode))
                {
                    _logger.LogError("Không thể xác định mã đơn hàng từ VNPay response và session");
                    TempData["Message"] = "Không thể xác định mã đơn hàng";
                    return RedirectToAction("PaymentFail");
                }
                
                // Lấy khách hàng đã chọn từ session
                var selectedKhachHangStr = HttpContext.Session.GetString("VnPaySelectedCustomerId");
                if (!int.TryParse(selectedKhachHangStr, out var selectedKhachHang))
                {
                    selectedKhachHang = HttpContext.Session.GetInt32("SelectedCustomerId") ?? 0;
                }
                
                if (selectedKhachHang <= 0)
                {
                    _logger.LogError("Không tìm thấy thông tin khách hàng đã chọn");
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin Khách Hàng đã chọn.";
                    return RedirectToAction("CheckOut");
                }

                var selectedKhachHangObject = db.KhachHangs.Find(selectedKhachHang);
                
                if (selectedKhachHangObject != null)
                {
                    // Lấy lại thông tin đơn hàng từ Session
                    var amount = response.Amount;
                    if (amount <= 0)
                    {
                        // Thử lấy từ session nếu response không có amount
                        var amountStr = HttpContext.Session.GetString("VnPayTotalAmount");
                        if (!string.IsNullOrEmpty(amountStr) && double.TryParse(amountStr, out var sessionAmount))
                        {
                            amount = sessionAmount;
                            _logger.LogInformation($"Sử dụng số tiền từ session: {amount}");
                        }
                    }
                    
                    _logger.LogInformation($"Bắt đầu tạo đơn hàng VNPay: MaDH={orderCode}, TongTien={amount}");
                    
                    // Thực hiện các bước thanh toán với selectedKhachHangObject
                    var gioHang = HttpContext.Session.GetJson<GioHang>("giohang");
                    var donHang = new DonHang
                    {
                        MaDonHang = orderCode,
                        NgayDatHang = DateTime.Now,
                        TongTien = amount,
                        TrangThaiDonHang = 1, 
                        MaKhachHang = selectedKhachHangObject.MaKhachHang,
                        ThanhToan = true, 
                        VanChuyen = 1,
                        PhiVanChuyen = 10000,
                        TienGiam = 0,
                        TongThanhToan = amount,
                        PaymentId = response.TransactionId, // Sử dụng mã giao dịch VNPay
                        PaymentMethod = "VNPay"
                    };

                    db.DonHangs.Add(donHang);
                    db.SaveChanges();

                    foreach (var line in gioHang.Lines)
                    {
                        // Lấy thông tin sản phẩm từ cơ sở dữ liệu
                        var sanPham = db.SanPhams.Find(line.SanPham.MaSanPham);

                        if (sanPham != null)
                        {
                            // Trừ số lượng đã mua từ số lượng tồn kho
                            sanPham.Slkho -= line.SoLuong;


                            // Cập nhật giá trị mới của số lượng tồn kho trong cơ sở dữ liệu
                            db.Update(sanPham);

                        }
                    }
                    db.SaveChanges();
                    foreach (var line in gioHang.Lines)
                    {
                        var chiTietDonHang = new ChiTietDonHang
                        {
                            MaDonHang = donHang.MaDonHang,
                            MaSanPham = line.SanPham.MaSanPham,
                            SoLuong = line.SoLuong,
                            GiaBan = line.SanPham.Gia
                        };
                        db.ChiTietDonHangs.Add(chiTietDonHang);
                        RemoveFromGioHang(line.SanPham.MaSanPham);

                    }
                    db.SaveChanges();


                    foreach (var line in gioHang.Lines)
                    {

                        RemoveFromGioHang(line.SanPham.MaSanPham);
                    }
                    gioHang.Clear();
                    
                    // Xóa dữ liệu thanh toán VNPay từ session
                    HttpContext.Session.Remove("VnPayOrderCode");
                    HttpContext.Session.Remove("VnPayTotalAmount");
                    HttpContext.Session.Remove("VnPaySelectedCustomerId");

                    _logger.LogInformation("Thanh toán VNPay thành công");
                    TempData["Message"] = $"Thanh toán VNPay thành công!";

                    // tạo shipment GHN
                    try
                    {
                        var shipmentReq = new ShipmentRequest
                        {
                            OrderCode = donHang.MaDonHang,
                            FromDistrictId = 1450,
                            ToDistrictId = 1450,
                            ToWardCode = "10000", // Mã phường xã mặc định
                            ToName = selectedKhachHangObject.TenKhachHang,
                            ToPhone = selectedKhachHangObject.SoDienThoai,
                            ToAddress = selectedKhachHangObject.DiaChi,
                            Weight = gioHang.TotalWeightGram(),
                            OrderValue = (decimal)donHang.TongThanhToan!
                        };
                        var providerCode = _shippingService.CreateShipmentAsync(shipmentReq).Result;
                        var shipment = new Shipment
                        {
                            OrderId = donHang.MaDonHang,
                            ProviderId = 1,
                            ProviderOrderCode = providerCode,
                            ShippingFee = (decimal)donHang.PhiVanChuyen!,
                            Status = ShipmentStatus.Pending.ToString()
                        };
                        db.Shipments.Add(shipment);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Lỗi tạo vận đơn GHN");
                    }

                    return RedirectToAction("PaymentSuccess");
                }
                else
                {
                    _logger.LogError("Không tìm thấy thông tin Khách Hàng đã chọn");
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin Khách Hàng đã chọn.";
                    return RedirectToAction("CheckOut");
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                _logger.LogError($"Đã xảy ra lỗi trong quá trình thanh toán VNPay: {ex.Message}");
                TempData["Message"] = $"Đã xảy ra lỗi trong quá trình thanh toán: {ex.Message}";
                return RedirectToAction("PaymentFail");
            }
        }

        #region Paypal payment
        [Authorize]
        [HttpPost("/GioHang/create-paypal-order")]
        public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
        {
            try
            {
                // Kiểm tra và lấy giỏ hàng từ session
                var gioHang = HttpContext.Session.GetJson<GioHang>("giohang");
                if (gioHang == null || !gioHang.Lines.Any())
                {
                    _logger.LogError("Giỏ hàng trống khi tạo đơn hàng PayPal");
                    return BadRequest(new { message = "Giỏ hàng trống" });
                }
                
                // Kiểm tra khách hàng đã chọn hay chưa
                var selectedKhachHang = HttpContext.Session.GetInt32("SelectedCustomerId");
                if (!selectedKhachHang.HasValue)
                {
                    _logger.LogError("Chưa chọn địa chỉ giao hàng khi tạo đơn hàng PayPal");
                    return BadRequest(new { message = "Vui lòng chọn địa chỉ giao hàng" });
                }
                
                // Lấy thông tin người dùng đã đăng nhập
                var taikhoanID = HttpContext.Session.GetString("AccountId");
                if (string.IsNullOrEmpty(taikhoanID))
                {
                    _logger.LogError("Người dùng chưa đăng nhập khi tạo đơn hàng PayPal");
                    return BadRequest(new { message = "Vui lòng đăng nhập để tiếp tục" });
                }
                
                // Thông tin đơn hàng gửi qua Paypal (chuyển đổi sang USD)
                double tongTienVND = gioHang.ComputeTotalValues() + 10000;
                if (gioHang.TienGiam > 0)
                {
                    tongTienVND -= gioHang.TienGiam;
                }
                
                // Chuyển đổi từ VND sang USD (tỷ giá cố định 23500)
                double tongTienUSD = tongTienVND / 23500;
                var tongTien = tongTienUSD.ToString("0.00");
                
                // Log thông tin thanh toán để debug
                _logger.LogInformation($"Tạo đơn hàng PayPal: Tổng tiền VND={tongTienVND}, USD={tongTien}, Client ID={_paypalClient.ClientId}");
                
                // Tạo mã đơn hàng duy nhất
                var maDonHangThamChieu = "DH" + DateTime.Now.Ticks.ToString();
                var donViTienTe = "USD";
                
                try
                {
                    var response = await _paypalClient.CreateOrder(tongTien, donViTienTe, maDonHangThamChieu);
                    
                    if (response == null || string.IsNullOrEmpty(response.id))
                    {
                        _logger.LogError("Không nhận được response ID từ PayPal khi tạo đơn hàng");
                        return BadRequest(new { message = "Không thể tạo đơn hàng PayPal" });
                    }
                    
                    // Lưu mã đơn hàng vào session để sử dụng sau khi thanh toán
                    HttpContext.Session.SetString("PayPalOrderReference", maDonHangThamChieu);
                    HttpContext.Session.SetString("PayPalOrderId", response.id);
                    
                    _logger.LogInformation($"Đơn hàng PayPal đã được tạo thành công: OrderID={response.id}");
                    
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Lỗi khi gọi API PayPal: {ex.Message}");
                    return BadRequest(new { message = "Lỗi khi gọi API PayPal: " + ex.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi tạo đơn hàng PayPal: {ex.Message}");
                return BadRequest(new { message = ex.GetBaseException().Message });
            }
        }

        [Authorize]
        [HttpPost("/GioHang/capture-paypal-order")]
        public async Task<IActionResult> CapturePaypalOrder([FromBody] PaypalCaptureRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.OrderID))
                {
                    _logger.LogError($"OrderID không được để trống, request: {(request == null ? "null" : "not null")}");
                    return BadRequest(new { message = "OrderID không được để trống" });
                }
                
                _logger.LogInformation($"Bắt đầu xử lý thanh toán PayPal, OrderID: {request.OrderID}");
                
                try 
                {
                    var response = await _paypalClient.CaptureOrder(request.OrderID);
                    if (response == null || string.IsNullOrEmpty(response.id))
                    {
                        _logger.LogError($"Không nhận được phản hồi từ PayPal khi xử lý với OrderID: {request.OrderID}");
                        return BadRequest(new { message = "Không nhận được phản hồi từ PayPal" });
                    }
                    
                    _logger.LogInformation($"Đã nhận được phản hồi từ PayPal: ID={response.id}, Status={response.status}");
                    
                    // Lấy tài khoản ID từ session
                    var taikhoanID = HttpContext.Session.GetString("AccountId");
                    if (string.IsNullOrEmpty(taikhoanID) || !int.TryParse(taikhoanID, out var accountId))
                    {
                        return BadRequest(new { message = "Không thể xác định tài khoản người dùng" });
                    }
                    
                    // Lấy khách hàng đã chọn
                    var selectedKhachHang = HttpContext.Session.GetInt32("SelectedCustomerId");
                    if (!selectedKhachHang.HasValue)
                    {
                        return BadRequest(new { message = "Không tìm thấy thông tin khách hàng đã chọn" });
                    }
                    
                    var khachHang = db.KhachHangs.Find(selectedKhachHang.Value);
                    if (khachHang == null)
                    {
                        return BadRequest(new { message = "Không tìm thấy thông tin khách hàng" });
                    }
                    
                    // Xử lý lưu đơn hàng
                    var gioHang = HttpContext.Session.GetJson<GioHang>("giohang");
                    if (gioHang == null || !gioHang.Lines.Any())
                    {
                        return BadRequest(new { message = "Giỏ hàng trống" });
                    }
                    
                    // Lấy mã đơn hàng đã lưu trong session hoặc tạo mới
                    var orderReference = HttpContext.Session.GetString("PayPalOrderReference");
                    var orderCode = !string.IsNullOrEmpty(orderReference) ? orderReference : GenerateUniqueOrderCode();
                    
                    // Tính tổng tiền
                    var amount = gioHang.ComputeTotalValues() + 10000; // Phí vận chuyển
                    var tienGiam = HttpContext.Session.GetString("TongGiamGia");
                    double giaGiam = 0;
                    if (!string.IsNullOrEmpty(tienGiam))
                    {
                        double.TryParse(tienGiam, out giaGiam);
                    }
                    
                    _logger.LogInformation($"Tạo đơn hàng sau thanh toán PayPal: MaDH={orderCode}, TongTien={amount}, GiamGia={giaGiam}");
                    
                    // Tạo đơn hàng mới
                    var donHang = new DonHang
                    {
                        MaDonHang = orderCode,
                        NgayDatHang = DateTime.Now,
                        TongTien = amount,
                        TrangThaiDonHang = 1, // Đã thanh toán
                        MaKhachHang = khachHang.MaKhachHang,
                        ThanhToan = true,
                        VanChuyen = 1, // Chờ vận chuyển
                        PhiVanChuyen = 10000,
                        TienGiam = giaGiam,
                        TongThanhToan = amount - giaGiam,
                        PaymentId = response.id, // Lưu ID thanh toán từ PayPal
                        PaymentMethod = "PayPal"
                    };
                    
                    db.DonHangs.Add(donHang);
                    db.SaveChanges();
                    
                    _logger.LogInformation($"Đã lưu đơn hàng {orderCode} vào cơ sở dữ liệu");
                    
                    // Cập nhật số lượng tồn kho và tạo chi tiết đơn hàng
                    foreach (var line in gioHang.Lines)
                    {
                        var sanPham = db.SanPhams.Find(line.SanPham.MaSanPham);
                        if (sanPham != null)
                        {
                            // Cập nhật số lượng tồn kho và lượt mua
                            sanPham.Slkho -= line.SoLuong;
                            sanPham.LuotMua += line.SoLuong;
                            db.Update(sanPham);
                            
                            // Tạo chi tiết đơn hàng
                            var chiTietDonHang = new ChiTietDonHang
                            {
                                MaDonHang = donHang.MaDonHang,
                                MaSanPham = line.SanPham.MaSanPham,
                                SoLuong = line.SoLuong,
                                GiaBan = line.SanPham.Gia
                            };
                            db.ChiTietDonHangs.Add(chiTietDonHang);
                        }
                    }
                    
                    db.SaveChanges();
                    _logger.LogInformation($"Đã cập nhật số lượng tồn kho và tạo chi tiết đơn hàng");
                    
                    // Gửi email thông báo đơn hàng thành công
                    try
                    {
                        var user = _context.Accounts.FirstOrDefault(a => a.AccountId == accountId);
                        if (user != null)
                        {
                            var mailData = new MailData
                            {
                                ReceiverName = khachHang.TenKhachHang,
                                ReceiverEmail = user.AccountEmail,
                                Title = "Thông báo đặt hàng thành công qua PayPal",
                                Body = "Chào " + khachHang.TenKhachHang + ",\n\n"
                                    + "Đơn hàng của bạn đã được xác nhận và thanh toán thành công qua PayPal. Dưới đây là chi tiết đơn hàng:\n\n"
                                    + "Mã đơn hàng: " + donHang.MaDonHang + "\n"
                                    + "Ngày đặt hàng: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "\n"
                                    + "Mã thanh toán PayPal: " + response.id + "\n"
                                    + "Danh sách sản phẩm:\n"
                            };

                            foreach (var line in gioHang.Lines)
                            {
                                mailData.Body += "- " + line.SanPham.TenSanPham + " x " + line.SoLuong + ": " 
                                              + string.Format("{0:#,##0} VNĐ", line.SanPham.Gia * line.SoLuong) + "\n";
                            }

                            mailData.Body += "\nTổng tiền: " + string.Format("{0:#,##0} VNĐ", donHang.TongTien) + "\n"
                                          + "Phí vận chuyển: " + string.Format("{0:#,##0} VNĐ", donHang.PhiVanChuyen) + "\n";
                            
                            if (donHang.TienGiam > 0)
                            {
                                mailData.Body += "Giảm giá: " + string.Format("{0:#,##0} VNĐ", donHang.TienGiam) + "\n";
                            }
                            
                            mailData.Body += "Tổng thanh toán: " + string.Format("{0:#,##0} VNĐ", donHang.TongThanhToan) + "\n\n"
                                          + "Cảm ơn bạn đã mua hàng tại cửa hàng chúng tôi.\n"
                                          + "Chúng tôi sẽ giao hàng cho bạn trong thời gian sớm nhất.";

                            _mailService.SendMail(mailData);
                            _logger.LogInformation($"Đã gửi email xác nhận đơn hàng thành công đến {user.AccountEmail}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Lỗi gửi email xác nhận đơn hàng: {ex.Message}");
                        // Không ảnh hưởng đến quy trình thanh toán nếu gửi email thất bại
                    }
                    
                    // Xóa giỏ hàng
                    gioHang.Clear();
                    HttpContext.Session.SetJson("giohang", gioHang);
                    
                    // Xóa thông tin đơn hàng khỏi session
                    HttpContext.Session.Remove("PayPalOrderReference");
                    HttpContext.Session.Remove("PayPalOrderId");
                    HttpContext.Session.Remove("TongGiamGia");
                    HttpContext.Session.Remove("MaKM");
                    
                    _logger.LogInformation("Thanh toán PayPal thành công, mã giao dịch: " + response.id);
                    
                    // tạo shipment GHN
                    try
                    {
                        var shipmentReq = new ShipmentRequest
                        {
                            OrderCode = donHang.MaDonHang,
                            FromDistrictId = 1450,
                            ToDistrictId = 1450,
                            ToWardCode = "10000", // Mã phường xã mặc định
                            ToName = khachHang.TenKhachHang,
                            ToPhone = khachHang.SoDienThoai,
                            ToAddress = khachHang.DiaChi,
                            Weight = gioHang.TotalWeightGram(),
                            OrderValue = (decimal)donHang.TongThanhToan!
                        };
                        var providerCode = _shippingService.CreateShipmentAsync(shipmentReq).Result;
                        var shipment = new Shipment
                        {
                            OrderId = donHang.MaDonHang,
                            ProviderId = 1,
                            ProviderOrderCode = providerCode,
                            ShippingFee = (decimal)donHang.PhiVanChuyen!,
                            Status = ShipmentStatus.Pending.ToString()
                        };
                        db.Shipments.Add(shipment);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Lỗi tạo vận đơn GHN");
                    }

                    return Ok(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Lỗi khi gọi API PayPal capture: {ex.Message}");
                    return BadRequest(new { message = "Lỗi khi gọi API PayPal capture: " + ex.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi xử lý thanh toán PayPal: {ex.Message}");
                return BadRequest(new { message = ex.GetBaseException().Message });
            }
        }

        // Class để nhận dữ liệu từ PayPal
        public class PaypalCaptureRequest
        {
            public string OrderID { get; set; }
        }

        #endregion

    }
}