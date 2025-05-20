using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using MyPhamSoul.Helpper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyPhamSoul.Models;
using PagedList.Core;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace MyPhamSoul.Areas.Admin.Controllers
{
    [Area("Admin")]
   
    public class RolesController : Controller
    {
        private readonly _2025MyPhamContext _context;
        public INotyfService _notifyService { get; }

        public RolesController(_2025MyPhamContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/Roles
        public IActionResult Index(int? page, string? search = "", string? moTa="")
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;

            IQueryable<Role> query = _context.Roles
                .AsNoTracking();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.RoleName.Contains(search));
            }
            if (!string.IsNullOrEmpty(moTa))
            {
                query = query.Where(x => x.Description.Contains(moTa));
            }

   

            var lsProducts = query.OrderByDescending(x => x.RoleId).ToList();

            PagedList<Role> models = new PagedList<Role>(lsProducts.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentMoTa = moTa;
        

            return View(models);
        }
        public IActionResult Filtter(string? search, string? moTa)
        {
            var url = "/Admin/Roles?";

           

            if (!string.IsNullOrEmpty(search))
            {
                url += $"search={search}&";
            }

            if (!string.IsNullOrEmpty(moTa))
            {
                url += $"moTa={moTa}&";
            }

            // Loại bỏ dấu '&' cuối cùng nếu có
            if (url.EndsWith("&"))
            {
                url = url.Substring(0, url.Length - 1);
            }

            return Json(new { status = "success", redirectUrl = url });
        }

       

        // GET: Admin/Roles/Details/5
        public async Task<IActionResult> Details(int? id, int? MaID, string? search, int? page, string? moTa)
        {
            if (id == null || _context.Roles == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(m => m.RoleId == id);
            if (role == null)
            {
                return NotFound();
            }
            ViewBag.CurrentPage = page;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentMaID = MaID;
            ViewBag.CurrentMoTa = moTa;
            ViewBag.Currentid = id;
            return View(role);
        }
        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DetailsConfirmed(string? search, int? page, int? id, int? MaID, string? moTa)
        {
            if (_context.SanPhams == null)
            {
                return Problem("Entity set '_2025MyPhamContext.SanPhams' is null.");
            }
            // Sau khi xóa thành công, bạn có thể chuyển hướng trở lại trang chứa sản phẩm vừa xóa bằng cách truyền tham số `page`.
            // Nếu `page` không có giá trị, bạn có thể mặc định nó về một trang cụ thể (ví dụ: 1).
            return RedirectToAction("Index", new
            {
                page = page,
                MaID = MaID,
                search = search,
                moTa = moTa
            });
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create(int? id, int? MaID, string? search, int? page, string? moTa)
        {
            ViewBag.CurrentPage = page;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentMaID = MaID;
            ViewBag.CurrentMoTa = moTa;
            ViewBag.Currentid = id;
            return View();
        }

        // POST: Admin/Roles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( int? page, [Bind("RoleId,RoleName,Description")] Role role)
        {
            if (string.IsNullOrEmpty(role.RoleName))
            {
                ModelState.AddModelError("RoleName", "Xin hãy nhập tên của người dùng.");
            }

            if (string.IsNullOrEmpty(role.Description))
            {
                ModelState.AddModelError("Description", "Xin hãy điền phần mô tả");
            }

            if (ModelState.IsValid)
            {
                _context.Add(role);
                await _context.SaveChangesAsync();
                _notifyService.Success("Tạo mới thành công");
                return RedirectToAction("Index", new
                {
                    page = page,

                });
            }
            return View(role);
        }

        // GET: Admin/Roles/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, int? MaID, string? search, int? page, string? moTa)
        {
            if (id == null || _context.Roles == null)
            {
                return NotFound();
            }

            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            ViewBag.CurrentPage = page;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentMaID = MaID;
            ViewBag.CurrentMoTa = moTa;
            ViewBag.Currentid = id;

            return View(role);
        }

        // POST: Admin/Roles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int? MaID, string? search, int? page, string? moTa, [Bind("RoleId,RoleName,Description")] Role role)
        {
            if (id != role.RoleId)
            {
                return NotFound();
            }
            if (string.IsNullOrEmpty(role.RoleName))
            {
                ModelState.AddModelError("RoleName", "Xin hãy nhập tên của người dùng.");
            }

            if (string.IsNullOrEmpty(role.Description))
            {
                ModelState.AddModelError("Description", "Xin hãy điền phần mô tả");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(role);
                    await _context.SaveChangesAsync();
                    _notifyService.Success("Cập nhật thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoleExists(role.RoleId))
                    {
                        _notifyService.Success("Có lỗi xảy ra");
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", new
                {
                    search = search,
                    page = page,
                    moTa = moTa,
                    MaID = MaID
                });
            }
            return View(role);
        }
        [Authorize(Roles = "Admin")]
        // GET: Admin/Roles/Delete/5
        public async Task<IActionResult> Delete(int? id, int? MaID, string? search, int? page, string? moTa)
        {
            if (id == null || _context.Roles == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(m => m.RoleId == id);
            if (role == null)
            {
                return NotFound();
            }
            ViewBag.CurrentPage = page;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentMaID = MaID;
            ViewBag.CurrentMoTa = moTa;
            ViewBag.Currentid = id;
            return View(role);
        }

        // POST: Admin/Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id, int? MaID, string? search, int? page, string? moTa)
        {
            if (_context.Roles == null)
            {
                return Problem("Entity set '_2025MyPhamContext.Roles'  is null.");
            }
            var role = await _context.Roles.FindAsync(id);
            var account = await _context.Accounts.Where(p => p.AccountId == id).ToListAsync();
            _context.RemoveRange(account);
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            _notifyService.Success("Xóa quyền truy cập thành công");
            return RedirectToAction("Index", new
            {
                page = page,
                MaID = MaID,
                search = search,
                moTa = moTa
            });
        }
        private bool RoleExists(int id)
        {
          return (_context.Roles?.Any(e => e.RoleId == id)).GetValueOrDefault();
        }
    }
}
