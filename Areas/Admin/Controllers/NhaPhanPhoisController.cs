using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyPhamSoul.Models;
using PagedList.Core;

namespace MyPhamSoul.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NhaPhanPhoisController : Controller
    {
        private readonly _2025MyPhamContext _context;

        public NhaPhanPhoisController(_2025MyPhamContext context)
        {
            _context = context;
        }

        // GET: Admin/NhaPhanPhois
   
        public IActionResult Index(int? page, string? MaID = null, string search = "", double? minPrice = null, double? maxPrice = null)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;

            IQueryable<NhaPhanPhoi> query = _context.NhaPhanPhois;
             

            if (!string.IsNullOrEmpty(MaID))
            {
                query = query.Where(x => x.MaNhaPp.Contains(MaID));
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.TenNhaPp.Contains(search));
            }

            //if (minPrice != null)
            //{
            //    query = query.Where(x => x.NgayNhan >= minPrice);
            //}

            //if (maxPrice != null)
            //{
            //    query = query.Where(x => x.TongTien <= maxPrice);
            //}



            var lsDonHangs = query.OrderByDescending(x => x.MaNhaPp).ToList();

            PagedList<NhaPhanPhoi> models = new PagedList<NhaPhanPhoi>(lsDonHangs.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentMaID = MaID;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;

            //ViewData["KhachHang"] = new SelectList(_context.DanhMucSanPhams, "MaKhachHang", "TenKhachHang", MaKH);

            return View(models);
        }
        // GET: Admin/NhaPhanPhois/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.NhaPhanPhois == null)
            {
                return NotFound();
            }

            var nhaPhanPhoi = await _context.NhaPhanPhois
                .FirstOrDefaultAsync(m => m.MaNhaPp == id);
            if (nhaPhanPhoi == null)
            {
                return NotFound();
            }

            return View(nhaPhanPhoi);
        }

        // GET: Admin/NhaPhanPhois/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/NhaPhanPhois/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNhaPp,TenNhaPp,DiaChi,Sdt,Email")] NhaPhanPhoi nhaPhanPhoi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nhaPhanPhoi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nhaPhanPhoi);
        }

        // GET: Admin/NhaPhanPhois/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.NhaPhanPhois == null)
            {
                return NotFound();
            }

            var nhaPhanPhoi = await _context.NhaPhanPhois.FindAsync(id);
            if (nhaPhanPhoi == null)
            {
                return NotFound();
            }
            return View(nhaPhanPhoi);
        }

        // POST: Admin/NhaPhanPhois/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaNhaPp,TenNhaPp,DiaChi,Sdt,Email")] NhaPhanPhoi nhaPhanPhoi)
        {
            if (id != nhaPhanPhoi.MaNhaPp)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nhaPhanPhoi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhaPhanPhoiExists(nhaPhanPhoi.MaNhaPp))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(nhaPhanPhoi);
        }

        // GET: Admin/NhaPhanPhois/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.NhaPhanPhois == null)
            {
                return NotFound();
            }

            var nhaPhanPhoi = await _context.NhaPhanPhois
                .FirstOrDefaultAsync(m => m.MaNhaPp == id);
            if (nhaPhanPhoi == null)
            {
                return NotFound();
            }

            return View(nhaPhanPhoi);
        }

        // POST: Admin/NhaPhanPhois/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.NhaPhanPhois == null)
            {
                return Problem("Entity set '_2025MyPhamContext.NhaPhanPhois'  is null.");
            }
            var nhaPhanPhoi = await _context.NhaPhanPhois.FindAsync(id);
            if (nhaPhanPhoi != null)
            {
                _context.NhaPhanPhois.Remove(nhaPhanPhoi);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NhaPhanPhoiExists(string id)
        {
          return (_context.NhaPhanPhois?.Any(e => e.MaNhaPp == id)).GetValueOrDefault();
        }
    }
}
