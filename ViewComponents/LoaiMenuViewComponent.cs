using MyPhamSoul.Models;
using Microsoft.AspNetCore.Mvc;
using MyPhamSoul.Repository;
using MyPhamSoul.Infrastructure;

namespace MyPhamSoul.ViewComponents
{
    public class LoaiMenuViewComponent: ViewComponent
    {
        private readonly ILoaiRepository _loai;
        public LoaiMenuViewComponent(ILoaiRepository loaiRepository)
        {
            _loai = loaiRepository;
        }
        public IViewComponentResult Invoke()
        {
            var loai = _loai.GetAllLoai().OrderBy(x => x.TenLoai);
            return View(loai);
        }
    }
    public class HangMenuViewComponent : ViewComponent
    {
        private readonly IHangRepository _hang;
        public HangMenuViewComponent(IHangRepository hangRepository)
        {
            _hang = hangRepository;
        }
        public IViewComponentResult Invoke()
        {
            var hang = _hang.GetAllHang().OrderBy(x => x.TenHang);
            return View(hang);
        }
    }

    public class CTLoaiMenuViewComponent : ViewComponent
    {
        private readonly ICTLoaiRepository _ctLoaiRepository;

        public CTLoaiMenuViewComponent(ICTLoaiRepository ctLoaiRepository)
        {
            _ctLoaiRepository = ctLoaiRepository;
        }

        public IViewComponentResult Invoke(string viewName, string maLoai)
        {
            var ctloais = _ctLoaiRepository.GetCtLoaiByLoai(maLoai).OrderBy(ct => ct.TenCtloai);
            return View(viewName, ctloais);
        }
    }

    public class GioHangWidget: ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View(HttpContext.Session.GetJson<GioHang>("giohang"));
        }
    }

    


}
