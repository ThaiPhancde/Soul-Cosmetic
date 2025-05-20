using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MyPhamSoul.Models;
using System.Diagnostics;
using System.Web;
using X.PagedList;
using static MyPhamSoul.Controllers.HomeController;

namespace MyPhamSoul.Controllers
{
  

    public class HomeController : Controller
    {
        _2025MyPhamContext db = new _2025MyPhamContext();

        private readonly _2025MyPhamContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HomeController(ILogger<HomeController> logger, _2025MyPhamContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;

        }

        public IActionResult Index()
        {

            var listDanhMucSanPham = db.DanhMucSanPhams.ToList();
            return View(listDanhMucSanPham);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //public async Task<IActionResult> DanhMucSanPham(int? page, string sortOrder)
        //{
        //    // Số sản phẩm trên mỗi trang
        //    int pageSize = 6;

        //    var danhMucSanPhams = db.DanhMucSanPhams.ToList();
        //    // Sắp xếp danh sách nếu sortOrder được chỉ định
        //    if (!string.IsNullOrEmpty(sortOrder))
        //    {
        //        switch (sortOrder)
        //        {
        //            case "average_rating":
        //                danhMucSanPhams = danhMucSanPhams.OrderBy(s => s.DanhGia).ToList();
        //                break;
        //            ;
        //            case "price_low_high":
        //                danhMucSanPhams = danhMucSanPhams.OrderBy(s => s.Gia).ToList();
        //                break;
        //            case "price_high_low":
        //                danhMucSanPhams = danhMucSanPhams.OrderByDescending(s => s.Gia).ToList();
        //                break;
        //            case "name_z":
        //                danhMucSanPhams = danhMucSanPhams.OrderByDescending(s => s.TenDanhMuc).ToList();
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //    // Sử dụng thư viện PagedList để phân trang
        //    IPagedList<DanhMucSanPham> pagedList = await danhMucSanPhams.ToPagedListAsync(page ?? 1, pageSize);

        //    return View(pagedList);
        //}

        //[HttpGet]
        //public IActionResult FilterByPriceAndTag(double minPrice, double maxPrice, string tag)
        //{
        //    // Bắt đầu với tất cả sản phẩm
        //    IQueryable<DanhMucSanPham> filteredSanPhams = db.DanhMucSanPhams;

        //    // Lọc sản phẩm dựa trên khoảng giá
        //    filteredSanPhams = filteredSanPhams.Where(p => p.Gia >= minPrice && p.Gia <= maxPrice);

        //    // Lọc sản phẩm dựa trên thẻ (tag)
        //    if (!string.IsNullOrEmpty(tag))
        //    {
        //        filteredSanPhams = filteredSanPhams.Where(p => p.TenDanhMuc.Contains(tag));
        //    }

        //    // Lấy danh sách sản phẩm đã lọc
        //    var filteredProducts = filteredSanPhams.ToList();

        //    // Trả về kết quả dưới dạng partial view
        //    return PartialView("_ReturnHangs", filteredProducts);
        //}





        public IActionResult DanhMucSanPham(int? page, List<string> selectedMaHangs = null, List<string> selectedMaCTLoais = null, string search = "", double? minPrice = null, double? maxPrice = null, string sortBy = "")
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 6;

            IQueryable<DanhMucSanPham> query = db.DanhMucSanPhams.AsQueryable();

            if (selectedMaCTLoais != null && selectedMaCTLoais.Any())
            {
                query = query.Where(x => selectedMaCTLoais.Contains(x.MaCtloai));
            }

            if (selectedMaHangs != null && selectedMaHangs.Any())
            {
                query = query.Where(x => selectedMaHangs.Contains(x.MaHang));
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

            switch (sortBy)
            {
                case "price_asc":
                    query = query.OrderBy(x => x.Gia);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(x => x.Gia);
                    break;
                case "name_asc":
                    query = query.OrderBy(x => x.TenDanhMuc);
                    break;
                case "name_desc":
                    query = query.OrderByDescending(x => x.TenDanhMuc);
                    break;
                default:
                    query = query.OrderBy(x => x.MaDanhMuc);
                    break;
            }

            var lsDanhMuc = query.ToList();

            PagedList<DanhMucSanPham> models = new PagedList<DanhMucSanPham>(lsDanhMuc.AsQueryable(), pageNumber, pageSize);

            ViewBag.PageNumber = pageNumber;
            ViewBag.SelectedMaHangs = selectedMaHangs;
            ViewBag.SelectedMaCTLoais = selectedMaCTLoais ?? new List<string>();
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.ListHangs = db.Hangs.Select(x => x.MaHang).ToList();
            ViewBag.ListCTLoais = db.Ctloais.Select(x => x.MaCtloai).ToList();


            return View(models);
        }

        public IActionResult Filtter(string selectedMaHangs = "", string selectedMaCTLoai = "", string search = "", double? minPrice = null, double? maxPrice = null, string sortBy = "")
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            if (!string.IsNullOrEmpty(selectedMaHangs))
            {
                queryString["selectedMaHangs"] = selectedMaHangs;
            }

            if (!string.IsNullOrEmpty(selectedMaCTLoai))
            {
                queryString["selectedMaCTLoai"] = selectedMaCTLoai;
            }

            if (!string.IsNullOrEmpty(search))
            {
                queryString["search"] = search;
            }

            if (minPrice != null)
            {
                queryString["minPrice"] = minPrice.ToString();
            }

            if (maxPrice != null)
            {
                queryString["maxPrice"] = maxPrice.ToString();
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                queryString["sortBy"] = sortBy;
            }

            // Add page parameter to maintain pagination
            queryString["page"] = "1";

            var url = $"/DanhMucSanPham?{queryString}";

            return Json(new { status = "success", redirectUrl = url });
        }


        public async Task<IActionResult> SanPhamTheoHang(string mahang, int? page, string sortOrder)
        {
            // Số sản phẩm trên mỗi trang
            int pageSize = 9;

            // Lấy danh sách sản phẩm theo mã hàng
            var danhMucSanPhams = db.DanhMucSanPhams
                .Include(x => x.MaHangNavigation) // Include để nạp thông tin về hãng
                .Where(x => x.MaHang == mahang);

            // Lấy tên hãng tương ứng với mã hàng
            var tenHang = danhMucSanPhams.FirstOrDefault()?.MaHangNavigation?.TenHang;

            // Sử dụng thư viện PagedList để phân trang
            IPagedList<DanhMucSanPham> pagedList = await danhMucSanPhams.ToPagedListAsync(page ?? 1, pageSize);

            // Đặt giá trị "mahang" vào ViewBag để truyền sang view
            ViewBag.Mahang = mahang;

            // Đặt giá trị tên hãng vào ViewBag
            ViewBag.TenHang = tenHang;

            return View(pagedList);
        }


        public IActionResult SanPhamTheoDanhMuc(string maDanhMuc)
        {
            var danhMuc = db.DanhMucSanPhams.FirstOrDefault(d => d.MaDanhMuc == maDanhMuc);
            var danhMucList = db.DanhMucSanPhams.Where(d => d.MaCtloai == danhMuc.MaCtloai).ToList();

            if (danhMuc == null)
            {
                return NotFound(); // Xử lý trường hợp danh mục không tồn tại
            }

            var sanPhamList = db.SanPhams.Where(s => s.MaDanhMuc == maDanhMuc).ToList();

            // Lấy danh sách màu sắc duy nhất từ danh sách sản phẩm
            var mauSanPhamList = sanPhamList.Select(s => s.Mau).Distinct().ToList();

            ViewData["DanhMuc"] = danhMuc;
            ViewData["DanhMucList"] = danhMucList;
            ViewData["MauSanPhamList"] = mauSanPhamList; // Truyền danh sách màu vào view
            return View(sanPhamList);
        }




        public IActionResult Chat()
        {
            var fullName = HttpContext.Session.GetString("AccountId");
            if (fullName != null)
            {
                int accountId;
                int.TryParse(fullName, out accountId);

                var user = _context.Accounts.FirstOrDefault(a => a.AccountId == accountId);
                ViewBag.UserName = user.FullName;
                return View();
            }
            else
            {
                ViewBag.UserName = "Khách hàng";
                return View();
            }
                
        }

		public IActionResult DetailsReview(int id)
		{
			var menuItem = _context.DanhGias
				.FirstOrDefault(m => m.MaDanhGia == id);

			if (menuItem == null)
			{
				return NotFound();
			}

			ViewBag.MenuItem = menuItem; // Truyền dữ liệu món ăn vào ViewBag
			return View(menuItem);
		}

        [HttpPost]
        public async Task<IActionResult> SubmitReview(string maMon, string content, int stars,
    IFormFile image1, IFormFile image2, IFormFile image3, IFormFile image4, IFormFile image5,
    IFormFile video)
        {

            // Kiểm tra xem MaMon có tồn tại trong CSDL không
            var product = _context.DanhMucSanPhams.FirstOrDefault(p => p.MaDanhMuc == maMon);
            if (product == null)
            {
                // Trả về một lỗi nếu không tìm thấy sản phẩm với MaMon đã cung cấp
                return NotFound();
            }

           
                // Trả về một lỗi nếu điểm đánh giá bằng 0 hoặc nhỏ hơn
                ModelState.AddModelError("stars", "Điểm đánh giá không được bằng 0.");
                // Có thể chuyển hướng người dùng về trang trước hoặc trang chi tiết sản phẩm với thông báo lỗi
               
           

            

            // Lấy thư mục lưu trữ ảnh và video
            string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

            // Tạo thư mục lưu trữ nếu chưa tồn tại
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            // Lưu hình ảnh vào thư mục lưu trữ và gán tên file cho các hình ảnh
            string image1FileName = await SaveImage(image1, uploadDir);
            string image2FileName = await SaveImage(image2, uploadDir);
            string image3FileName = await SaveImage(image3, uploadDir);
            string image4FileName = await SaveImage(image4, uploadDir);
            string image5FileName = await SaveImage(image5, uploadDir);

            // Lưu video vào thư mục lưu trữ nếu có
            string videoFileName = await SaveVideo(video, uploadDir);
            var taikhoanID = HttpContext.Session.GetString("AccountId");
            int id;
            int.TryParse(taikhoanID, out id);
            var hoTen = _context.Accounts.Where( h => h.AccountId == id ).FirstOrDefault();
            // Tạo một đối tượng Đánh giá mới

            if (stars != 0)
            {
                var danhGia = new DanhGia
                {
                    MaDanhGia = GenerateRandomMaDanhGia(), // Tạo số ngẫu nhiên có 4 chữ số
                    MaDanhMuc = maMon, // Lấy MaMon từ sản phẩm
                    HoTen = hoTen.FullName, // Lấy Tên đăng nhập từ phiên đăng nhập hiện tại
                    NoiDung = content, // Lấy nội dung từ form
                    NgayDG = DateTime.Now, // Lấy ngày hiện tại
                    Diem = stars, // Lấy điểm từ form
                    HinhAnh1 = GetFileNameOrDefault(image1FileName),
                    HinhAnh2 = GetFileNameOrDefault(image2FileName),
                    HinhAnh3 = GetFileNameOrDefault(image3FileName),
                    HinhAnh4 = GetFileNameOrDefault(image4FileName),
                    HinhAnh5 = GetFileNameOrDefault(image5FileName),
                    Video = GetFileNameOrDefault(videoFileName)
                };

                // Thêm đánh giá mới vào trong CSDL
                _context.DanhGias.Add(danhGia);
                _context.SaveChanges();

                // Sau khi thêm đánh giá, bạn có thể chuyển hướng người dùng đến trang chi tiết sản phẩm hoặc thực hiện hành động khác
                return Redirect($"/Home/SanPhamTheoDanhMuc?maDanhMuc={maMon}");
            }
            else
            {
                ModelState.AddModelError("stars", "Điểm đánh giá không được bằng 0.");
                return Redirect($"/Home/SanPhamTheoDanhMuc?maDanhMuc={maMon}");
            }


        }

        private async Task<string> SaveImage(IFormFile image, string uploadDir)
        {
            if (image != null && image.Length > 0)
            {
                // Tạo tên file mới
                string imageFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                string imagePath = Path.Combine(uploadDir, imageFileName);

                // Lưu tệp vào thư mục lưu trữ
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                return imageFileName;
            }

            // Trả về giá trị null nếu không có hình ảnh được tải lên
            return null;
        }

        private async Task<string> SaveVideo(IFormFile video, string uploadDir)
        {
            if (video != null && video.Length > 0)
            {
                // Tạo tên file mới
                string videoFileName = Guid.NewGuid().ToString() + Path.GetExtension(video.FileName);
                string videoPath = Path.Combine(uploadDir, videoFileName);

                // Lưu tệp vào thư mục lưu trữ
                using (var stream = new FileStream(videoPath, FileMode.Create))
                {
                    await video.CopyToAsync(stream);
                }

                return videoFileName;
            }

            // Trả về giá trị null nếu không có video được tải lên
            return null;
        }

        private string GetFileNameOrDefault(string fileName)
        {
            return fileName ?? null;
        }



        // Phương thức này sẽ tạo số ngẫu nhiên có 4 chữ số và chưa tồn tại trong CSDL
        private int GenerateRandomMaDanhGia()
        {
            Random random = new Random();
            int randomNumber;
            do
            {
                randomNumber = random.Next(1000, 9999); // Tạo số ngẫu nhiên có 4 chữ số
            } while (_context.DanhGias.Any(dg => dg.MaDanhGia == randomNumber)); // Kiểm tra xem số này đã tồn tại trong CSDL chưa
            return randomNumber;
        }

    }


}
