using Microsoft.AspNetCore.Mvc;

namespace MyPhamSoul.Areas.Admin.Controllers
{
    public class DangNhapController : Controller
    {
        [Area("Admin")]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
    }
}
