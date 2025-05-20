using Microsoft.AspNetCore.Mvc;
using MyPhamSoul.Models;
using System.Collections.Generic;

namespace MyPhamCheilinus.Controllers
{
    public class VirtualServicesController : Controller
    {
        private readonly _2025MyPhamContext _context;

        public VirtualServicesController(_2025MyPhamContext context)
        {
            _context = context;
        }

        // GET: VirtualServices
        public IActionResult Index()
        {
            // Dữ liệu mẫu cho virtual services
            var services = new List<object>
            {
                new {
                    Id = 1,
                    Title = "TƯ VẤN TRỰC TUYẾN",
                    Description = "Nhận tư vấn riêng từ chuyên gia của chúng tôi để tìm ra sản phẩm hoàn hảo cho làn da của bạn.",
                    ImageUrl = "/images/blog/blog-grid-home-1-img-1.jpg",
                    ButtonText = "Đặt lịch ngay",
                    Url = "/VirtualServices/Consultation"
                },
                new {
                    Id = 2,
                    Title = "LỚP HỌC TRANG ĐIỂM TRỰC TUYẾN",
                    Description = "Học các kỹ thuật trang điểm chuyên nghiệp từ chuyên gia trang điểm của chúng tôi qua lớp học trực tuyến.",
                    ImageUrl = "/images/blog/blog-grid-home-1-img-2.jpg",
                    ButtonText = "Tham gia ngay",
                    Url = "/VirtualServices/MakeupClass"
                },
                new {
                    Id = 3,
                    Title = "THỬ MỸ PHẨM ẢO",
                    Description = "Dùng công nghệ AR để thử các màu son, phấn mắt và các sản phẩm khác trước khi mua hàng.",
                    ImageUrl = "/images/blog/blog-grid-home-1-img-3.jpg",
                    ButtonText = "Thử ngay",
                    Url = "/VirtualServices/VirtualTryOn"
                },
                new {
                    Id = 4,
                    Title = "PHÂN TÍCH DA",
                    Description = "Nhận phân tích chi tiết về tình trạng da của bạn và các đề xuất sản phẩm được cá nhân hóa.",
                    ImageUrl = "/images/blog/blog-grid-home-1-img-4.jpg",
                    ButtonText = "Bắt đầu phân tích",
                    Url = "/VirtualServices/SkinAnalysis"
                }
            };

            // Danh sách câu hỏi thường gặp
            var faqs = new List<object>
            {
                new {
                    Question = "Làm thế nào để đặt lịch tư vấn trực tuyến?",
                    Answer = "Bạn có thể đặt lịch tư vấn trực tuyến bằng cách nhấp vào nút 'Đặt lịch ngay' trên trang Tư vấn trực tuyến, chọn ngày và giờ phù hợp, và điền thông tin liên hệ của bạn."
                },
                new {
                    Question = "Các lớp học trang điểm trực tuyến có phí không?",
                    Answer = "Chúng tôi có cả lớp học miễn phí và có phí. Các lớp học cơ bản thường miễn phí, trong khi các khóa học chuyên sâu có thể có phí. Chi tiết về học phí sẽ được hiển thị rõ ràng trước khi bạn đăng ký."
                },
                new {
                    Question = "Tôi cần thiết bị gì để tham gia lớp học trang điểm trực tuyến?",
                    Answer = "Bạn chỉ cần một thiết bị có camera (điện thoại thông minh, máy tính bảng hoặc laptop) và kết nối internet ổn định. Nếu có thể, hãy chuẩn bị sẵn các sản phẩm trang điểm cơ bản để thực hành theo."
                },
                new {
                    Question = "Tính năng thử mỹ phẩm ảo hoạt động như thế nào?",
                    Answer = "Tính năng này sử dụng công nghệ AR (Thực tế tăng cường) để mô phỏng việc sử dụng các sản phẩm trang điểm trên khuôn mặt của bạn thông qua camera. Bạn có thể xem trước nhiều màu sắc và sản phẩm khác nhau mà không cần thử trực tiếp."
                }
            };

            // Truyền dữ liệu đến view
            ViewBag.Services = services;
            ViewBag.FAQs = faqs;
            return View();
        }

        // GET: VirtualServices/Consultation
        public IActionResult Consultation()
        {
            return View();
        }

        // GET: VirtualServices/MakeupClass
        public IActionResult MakeupClass()
        {
            // Danh sách các lớp học mẫu
            var classes = new List<object>
            {
                new {
                    Id = 1,
                    Title = "Trang điểm cơ bản cho người mới bắt đầu",
                    Description = "Học các kỹ thuật trang điểm cơ bản từ A-Z dành cho người mới.",
                    Instructor = "Phương Linh",
                    Duration = "60 phút",
                    Price = "Miễn phí",
                    Date = "Thứ 7, 15/07/2023",
                    Time = "10:00 - 11:00",
                    ImageUrl = "/images/blog/blog-grid-home-1-img-2.jpg"
                },
                new {
                    Id = 2,
                    Title = "Trang điểm mắt khói quyến rũ",
                    Description = "Hướng dẫn chi tiết cách tạo đôi mắt khói cuốn hút.",
                    Instructor = "An Thái",
                    Duration = "45 phút",
                    Price = "199.000đ",
                    Date = "Chủ nhật, 16/07/2023",
                    Time = "15:00 - 15:45",
                    ImageUrl = "/images/blog/blog-grid-home-1-img-3.jpg"
                },
                new {
                    Id = 3,
                    Title = "Bí quyết trang điểm bền màu mùa hè",
                    Description = "Các kỹ thuật và sản phẩm giúp lớp trang điểm bền màu suốt ngày dài.",
                    Instructor = "Phương Linh",
                    Duration = "60 phút",
                    Price = "249.000đ",
                    Date = "Thứ 4, 19/07/2023",
                    Time = "19:00 - 20:00",
                    ImageUrl = "/images/blog/blog-grid-home-1-img-1.jpg"
                }
            };

            ViewBag.Classes = classes;
            return View();
        }

        // GET: VirtualServices/VirtualTryOn
        public IActionResult VirtualTryOn()
        {
            // Danh sách sản phẩm có thể thử
            var products = new List<object>
            {
                new {
                    Id = 1,
                    Name = "Son môi lâu trôi Soul",
                    Description = "Son lì mịn môi, lâu trôi, không khô môi",
                    Colors = new List<string> { "#FF5252", "#FF8A80", "#D81B60", "#C2185B", "#880E4F" },
                    Category = "Son môi",
                    ImageUrl = "/images/blog/blog-grid-home-1-img-5.jpg"
                },
                new {
                    Id = 2,
                    Name = "Phấn mắt Soul Palette",
                    Description = "Bảng phấn mắt 12 màu với kết cấu mềm mịn, dễ tán",
                    Colors = new List<string> { "#FFD54F", "#4FC3F7", "#81C784", "#9575CD", "#F06292" },
                    Category = "Phấn mắt",
                    ImageUrl = "/images/blog/blog-grid-home-1-img-3.jpg"
                },
                new {
                    Id = 3,
                    Name = "Má hồng Soul Blush",
                    Description = "Má hồng dạng kem cho vẻ ửng hồng tự nhiên",
                    Colors = new List<string> { "#FFAB91", "#F48FB1", "#CE93D8", "#90CAF9", "#80DEEA" },
                    Category = "Má hồng",
                    ImageUrl = "/images/blog/blog-grid-home-1-img-4.jpg"
                }
            };

            ViewBag.Products = products;
            return View();
        }

        // GET: VirtualServices/SkinAnalysis
        public IActionResult SkinAnalysis()
        {
            return View();
        }
    }
} 