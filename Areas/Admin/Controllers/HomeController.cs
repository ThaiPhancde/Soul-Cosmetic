using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyPhamSoul.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Employee, Admin")]
    public class HomeController : Controller
    {
      
        public IActionResult Index()
        {
            return View();
        }
    }
}
