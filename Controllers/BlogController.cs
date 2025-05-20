using Microsoft.AspNetCore.Mvc;
using MyPhamSoul.Models;
using System.Collections.Generic;
using System.Linq;

namespace MyPhamCheilinus.Controllers
{
    public class BlogController : Controller
    {
        private readonly _2025MyPhamContext _context;

        public BlogController(_2025MyPhamContext context)
        {
            _context = context;
        }

        // GET: Blog
        public IActionResult Index()
        {
            // Tạo danh sách bài viết mẫu
            var blogPosts = new List<object>
            {
                new {
                    Id = 1,
                    Title = "Xu hướng làm đẹp mùa hè 2025",
                    Summary = "Khám phá những xu hướng làm đẹp hot nhất mùa hè 2025 để làm mới diện mạo của bạn",
                    ImageUrl = "/images/DanhMucSanPham/Bảng Phấn Mắt 10 Ô Siêu Lấp Lánh Romand Better Than Eye Palette.jpg",
                    Author = "Phương Linh",
                    Date = "15/06/2025",
                    Category = "Xu hướng"
                },
                new {
                    Id = 2,
                    Title = "Cách chăm sóc da mụn hiệu quả tại nhà",
                    Summary = "Những bí quyết giúp bạn kiểm soát làn da mụn và phục hồi da khỏe mạnh một cách tự nhiên",
                    ImageUrl = "/images/DanhMucSanPham/Bút Kẻ Mắt Nước Chống Trôi Merzy The First Pen Eyeliner.jpg",
                    Author = "An Thái",
                    Date = "10/06/2025",
                    Category = "Chăm sóc da"
                },
                new {
                    Id = 3,
                    Title = "Hướng dẫn trang điểm cho người mới bắt đầu",
                    Summary = "Những bước cơ bản giúp bạn trang điểm đẹp tự nhiên dù là người mới làm quen với mỹ phẩm",
                    ImageUrl = "/images/DanhMucSanPham/Kem Nền Tươi Mướt, Chống Nắng Bảo Vệ Da Maybelline New York Fit Me Fresh Tint.jpg",
                    Author = "Phương Linh",
                    Date = "05/06/2025",
                    Category = "Trang điểm"
                },
                new {
                    Id = 4,
                    Title = "Top 5 sản phẩm skincare không thể thiếu",
                    Summary = "Những sản phẩm chăm sóc da cơ bản mà bất kỳ ai cũng nên có trong quy trình làm đẹp hàng ngày",
                    ImageUrl = "/images/DanhMucSanPham/Kem Lót Làm Mịn Da, Che Khuyết Điểm, Se Khít Lỗ Chân Lông Maybelline Baby Skin Pore Eraser.jpg",
                    Author = "An Thái",
                    Date = "01/06/2025", 
                    Category = "Skincare"
                },
                new {
                    Id = 5,
                    Title = "Cách chọn son môi phù hợp với tông da",
                    Summary = "Bí quyết chọn màu son hoàn hảo dựa trên tông da và sắc tố của bạn",
                    ImageUrl = "/images/DanhMucSanPham/Son Kem Lì Cực Nhẹ Môi Romand Zero Velvet Tint.jpg",
                    Author = "Phương Linh",
                    Date = "28/05/2025",
                    Category = "Trang điểm"
                },
                new {
                    Id = 6,
                    Title = "Cách dưỡng tóc mềm mượt tại nhà",
                    Summary = "Những nguyên liệu tự nhiên và phương pháp hiệu quả giúp mái tóc của bạn chắc khỏe, suôn mượt",
                    ImageUrl = "/images/DanhMucSanPham/Maybelline Fit Me Matte Poreless Powder SPF32 PA+++.jpg",
                    Author = "An Thái",
                    Date = "25/05/2025",
                    Category = "Chăm sóc tóc"
                }
            };

            // Danh sách danh mục blog
            var categories = new List<string> { "Tất cả", "Xu hướng", "Chăm sóc da", "Trang điểm", "Skincare", "Chăm sóc tóc" };

            // Truyền dữ liệu đến view
            ViewBag.Categories = categories;
            return View(blogPosts);
        }

        // GET: Blog/Detail/5
        public IActionResult Detail(int id)
        {
            // Trong thực tế sẽ lấy dữ liệu từ database
            var post = new
            {
                Id = id,
                Title = "Xu hướng làm đẹp mùa hè 2025",
                Content = "<p>Mùa hè đã đến, và cùng với đó là những xu hướng làm đẹp mới nhất đang làm mưa làm gió trong giới mộ điệu. Từ lớp trang điểm tự nhiên, rạng rỡ đến những màu son tươi tắn, hãy cùng chúng tôi khám phá những xu hướng làm đẹp nổi bật nhất mùa hè năm nay.</p>" +
                          "<h3>1. Trang điểm tự nhiên với làn da căng bóng</h3>" +
                          "<p>Xu hướng \"glass skin\" tiếp tục thống trị mùa hè này với lớp nền mỏng nhẹ, trong suốt, tạo cảm giác da căng bóng, khỏe mạnh. Thay vì lớp phấn dày, hãy sử dụng kem nền dạng serum hoặc kem dưỡng ẩm có màu (tinted moisturizer) để làn da trông tự nhiên nhưng vẫn rạng rỡ.</p>" +
                          "<h3>2. Màu mắt pastel</h3>" +
                          "<p>Các tông màu pastel nhẹ nhàng như xanh bạc hà, hồng phấn, và tím lavender đang trở thành lựa chọn phổ biến cho phấn mắt mùa hè. Những gam màu này không chỉ mang lại vẻ ngọt ngào, nữ tính mà còn giúp đôi mắt trông tươi tắn, rạng rỡ hơn.</p>" +
                          "<h3>3. Son môi glossy</h3>" +
                          "<p>Son bóng đang quay trở lại mạnh mẽ trong mùa hè này, thay thế cho xu hướng son lì những năm trước. Những gam màu cam đào, hồng coral sẽ giúp đôi môi trông căng mọng, tươi tắn, phù hợp với không khí rạng rỡ của mùa hè.</p>" +
                          "<h3>4. Má hồng dạng kem</h3>" +
                          "<p>Má hồng dạng kem đang được ưa chuộng hơn cả nhờ khả năng tạo hiệu ứng ửng hồng tự nhiên, như vừa đi dạo ngoài nắng về. Đặc biệt, má hồng kem còn dễ tán đều và bền màu hơn trong thời tiết nóng ẩm.</p>" +
                          "<h3>5. Tóc nhuộm highlight</h3>" +
                          "<p>Mùa hè là thời điểm hoàn hảo để làm mới mái tóc với những điểm nhấn highlight. Các tông màu caramel, vàng mật và bạch kim đang rất được ưa chuộng, mang lại vẻ ngoài tươi sáng, trẻ trung.</p>",
                ImageUrl = "/images/DanhMucSanPham/Bảng Phấn Mắt 10 Ô Siêu Lấp Lánh Romand Better Than Eye Palette.jpg",
                Author = "Phương Linh",
                Date = "15/06/2025",
                Category = "Xu hướng",
                Tags = new List<string> { "mùa hè", "trang điểm", "skincare", "xu hướng", "làm đẹp" },
                RelatedPosts = new List<object>
                {
                    new { Id = 2, Title = "Cách chăm sóc da mụn hiệu quả tại nhà", ImageUrl = "/images/DanhMucSanPham/Bút Kẻ Mắt Nước Chống Trôi Merzy The First Pen Eyeliner.jpg" },
                    new { Id = 3, Title = "Hướng dẫn trang điểm cho người mới bắt đầu", ImageUrl = "/images/DanhMucSanPham/Kem Nền Tươi Mướt, Chống Nắng Bảo Vệ Da Maybelline New York Fit Me Fresh Tint.jpg" },
                    new { Id = 5, Title = "Cách chọn son môi phù hợp với tông da", ImageUrl = "/images/DanhMucSanPham/Son Kem Lì Cực Nhẹ Môi Romand Zero Velvet Tint.jpg" }
                }
            };

            return View(post);
        }

        // GET: Blog/Category/skincare
        public IActionResult Category(string name)
        {
            // Trong thực tế sẽ lấy dữ liệu từ database theo danh mục
            var blogPosts = new List<object>
            {
                new {
                    Id = 2,
                    Title = "Cách chăm sóc da mụn hiệu quả tại nhà",
                    Summary = "Những bí quyết giúp bạn kiểm soát làn da mụn và phục hồi da khỏe mạnh một cách tự nhiên",
                    ImageUrl = "/images/DanhMucSanPham/Bút Kẻ Mắt Nước Chống Trôi Merzy The First Pen Eyeliner.jpg",
                    Author = "An Thái",
                    Date = "10/06/2025",
                    Category = name
                },
                new {
                    Id = 4,
                    Title = "Top 5 sản phẩm skincare không thể thiếu",
                    Summary = "Những sản phẩm chăm sóc da cơ bản mà bất kỳ ai cũng nên có trong quy trình làm đẹp hàng ngày",
                    ImageUrl = "/images/DanhMucSanPham/Kem Lót Làm Mịn Da, Che Khuyết Điểm, Se Khít Lỗ Chân Lông Maybelline Baby Skin Pore Eraser.jpg",
                    Author = "An Thái",
                    Date = "01/06/2025",
                    Category = name
                }
            };

            // Danh sách danh mục blog
            var categories = new List<string> { "Tất cả", "Xu hướng", "Chăm sóc da", "Trang điểm", "Skincare", "Chăm sóc tóc" };

            // Truyền dữ liệu đến view
            ViewBag.Categories = categories;
            ViewBag.CurrentCategory = name;
            return View("Index", blogPosts);
        }
    }
} 