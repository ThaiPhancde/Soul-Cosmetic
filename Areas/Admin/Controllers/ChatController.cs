using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MyPhamSoul.Models;
using System.Diagnostics;
using System.Web;
using X.PagedList;
using static MyPhamSoul.Controllers.HomeController;
using Microsoft.AspNetCore.Authorization;


namespace MyPhamSoul.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee")]
    public class ChatController : Controller
    {


        private readonly _2025MyPhamContext _context;
        

        public ChatController(_2025MyPhamContext context)
        {
            _context = context;
            
        }

        public IActionResult Index()
        {         
            return View();
        }
    }
}
