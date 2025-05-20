using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using MyPhamSoul.Models;
using X.PagedList;

namespace ThanhThoaiRestaurant.ViewComponents
{
    public class DanhGiaViewComponent : ViewComponent
    {
        private readonly _2025MyPhamContext _context;

        public DanhGiaViewComponent(_2025MyPhamContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke(string maMon, int? page, int pageSize = 2)
        {
            // Lấy danh sách đánh giá tương ứng với MaMon và phân trang
            var reviews = _context.DanhGias
                .Where(dg => dg.MaDanhMuc == maMon)
                .OrderByDescending(dg => dg.NgayDG);

            int pageNumber = page ?? 1;
            var pagedList = reviews.ToPagedList(pageNumber, pageSize);


            int startItem = (pageNumber - 1) * pageSize + 1;
            int endItem = Math.Min(startItem + pageSize - 1, pagedList.TotalItemCount);

            int maxVisiblePages = Math.Min(pagedList.PageCount, 5); // Tối đa 5 trang, nhưng không nhiều hơn tổng số trang
            int startPage = Math.Max(1, pageNumber - (maxVisiblePages / 2));
            int endPage = Math.Min(pagedList.PageCount, startPage + maxVisiblePages - 1);

            ViewBag.TotalItems = pagedList.TotalItemCount;
            ViewBag.TotalPages = pagedList.PageCount;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.StartItem = startItem;
            ViewBag.EndItem = endItem;
            ViewBag.MaxVisiblePages = maxVisiblePages;
            ViewBag.StartPage = startPage;
            ViewBag.EndPage = endPage;
            ViewBag.StartPage = startPage;
            ViewBag.EndPage = endPage;
            return View(pagedList);
        }
    }
}
