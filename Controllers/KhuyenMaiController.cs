using Microsoft.AspNetCore.Mvc;
using MyPhamSoul.Models;


namespace ThanhThoaiRestaurant.Controllers
{
    public class KhuyenMaiController : Controller
    {

        private readonly _2025MyPhamContext _context;

        public KhuyenMaiController(_2025MyPhamContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var taikhoanID = HttpContext.Session.GetString("AccountId");
            int id;
            int.TryParse(taikhoanID, out id);
            if (taikhoanID != null)
            {

               

                var query = _context.KhuyenMais.Where(d => d.AccountId == id && d.NgayKT >= DateTime.Now).ToList();
                return View(query);
            }
            return RedirectToAction("Login", "Accounts");
        }
    }
}
