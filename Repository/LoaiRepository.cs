using Microsoft.EntityFrameworkCore;
using MyPhamSoul.Models;

namespace MyPhamSoul.Repository
{
    public class LoaiRepository : ILoaiRepository
    {
        private readonly _2025MyPhamContext _context;
        public LoaiRepository(_2025MyPhamContext context)
        {
            _context = context;
        }

        public Loai Add(Loai loai)
        {
            _context.Loais.Add(loai);
            _context.SaveChanges();
            return loai;
        }

        public Loai Delete(string maloai)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Loai> GetAllLoai()
        {
            return _context.Loais;
        }

        public Loai GetLoai(string maloai)
        {
            return _context.Loais.Find(maloai);
        }

        public Loai Update(Loai loai)
        {
            _context.Update(loai);
            _context.SaveChanges();
            return loai;
        }
    }



    public class HangRepository : IHangRepository
    {
        private readonly _2025MyPhamContext _context;
        public HangRepository(_2025MyPhamContext context)
        {
            _context = context;
        }

        public Hang Add(Hang hang)
        {
            _context.Hangs.Add(hang);
            _context.SaveChanges();
            return hang;
        }

        public Hang Delete(string mahang)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Hang> GetAllHang()
        {
            return _context.Hangs;
        }

        public Hang GetHang(string mahang)
        {
            return _context.Hangs.Find(mahang);
        }

        public Hang Update(Hang hang)
        {
            _context.Update(hang);
            _context.SaveChanges();
            return hang;
        }
    }

    public class CTLoaiRepository : ICTLoaiRepository
    {
        private readonly _2025MyPhamContext _context;

        public CTLoaiRepository(_2025MyPhamContext context)
        {
            _context = context;
        }

        public List<Ctloai> GetCtLoaiByLoai(string maLoai)
        {
            return _context.Ctloais
                .Where(ct => ct.MaLoai == maLoai)
                .ToList();
        }

        public Loai Add(Ctloai ctloai)
        {
            _context.Ctloais.Add(ctloai);
            _context.SaveChanges();

            // Trả về đối tượng Loai tương ứng nếu cần
            return _context.Loais.FirstOrDefault(l => l.MaLoai == ctloai.MaLoai);
        }

        public Loai Update(Ctloai ctloai)
        {
            _context.Ctloais.Update(ctloai);
            _context.SaveChanges();

            // nt
            return _context.Loais.FirstOrDefault(l => l.MaLoai == ctloai.MaLoai);
        }

        public Loai Delete(string mactloai)
        {
            var ctloai = _context.Ctloais.Find(mactloai);
            if (ctloai != null)
            {
                _context.Ctloais.Remove(ctloai);
                _context.SaveChanges();

                // nt
                return _context.Loais.FirstOrDefault(l => l.MaLoai == ctloai.MaLoai);
            }

            return null;
        }

        public Loai GetLoai(string mactloai)
        {
            var ctloai = _context.Ctloais.Find(mactloai);
            return ctloai?.MaLoaiNavigation;
        }

        public IEnumerable<Ctloai> GetAllCTLoai()
        {
            return _context.Ctloais.ToList();
        }
    }

   

}
