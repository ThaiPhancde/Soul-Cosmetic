using Microsoft.AspNetCore.Mvc;
using System.Linq;
using MyPhamSoul.Models;

namespace MyPhamSoul.ViewComponents
{
    public class RatingSummaryViewComponent : ViewComponent
    {
        private readonly _2025MyPhamContext _context;

        public RatingSummaryViewComponent(_2025MyPhamContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke(string maMon)
        {
            var ratings = _context.DanhGias.Where(dg => dg.MaDanhMuc == maMon).ToList();
            var ratingCounts = new int[5]; // Mảng lưu trữ số lượng sao cho mỗi đánh giá từ 1 đến 5
            int totalRatings = ratings.Count;

            foreach (var rating in ratings)
            {
                // Tăng số lượng sao tương ứng với đánh giá
                ratingCounts[rating.Diem - 1]++;
            }

            // Tính số sao trung bình
            double averageRating = 0;
            for (int i = 0; i < 5; i++)
            {
                averageRating += (i + 1) * ratingCounts[i];
            }
            averageRating = totalRatings > 0 ? averageRating / (totalRatings * 5) * 5 : 0;

            ViewBag.AverageRating = averageRating;

            return View(ratingCounts);
        }
    }
}
