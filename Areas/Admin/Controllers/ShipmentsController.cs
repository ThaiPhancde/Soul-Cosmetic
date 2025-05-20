using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPhamSoul.Models;
using Microsoft.AspNetCore.Authorization;

namespace MyPhamSoul.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Employee,Admin")]
    public class ShipmentsController : Controller
    {
        private readonly _2025MyPhamContext _db;
        public ShipmentsController(_2025MyPhamContext db) => _db = db;

        public IActionResult Index()
        {
            var list = _db.Shipments
                           .Include(s => s.Order)
                           .Where(s => s.Status == "Confirmed" || s.Status == "Delivering" || s.Status == "Pending")
                           .OrderByDescending(s => s.Id)
                           .ToList();
            return View(list);
        }
    }
} 