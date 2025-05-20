namespace MyPhamSoul.Models
{
    public class GioHang
    {
        public List<GioHangLine> Lines { get; set; } = new List<GioHangLine>();
        public void AddItem(SanPham sanpham, int soluong)
        {
            GioHangLine? line = Lines.Where(p => p.SanPham.MaSanPham == sanpham.MaSanPham).FirstOrDefault();
            if (line == null)
            {
                Lines.Add(new GioHangLine
                {
                    SanPham = sanpham,
                    SoLuong = soluong
                });
            }
            else
            {
                line.SoLuong += soluong;
            }
        }

        public void RemoveLine(SanPham sanpham) =>
            Lines.RemoveAll(l => l.SanPham.MaSanPham == sanpham.MaSanPham);

        public void UpdateQuantity(string maSanPham, int newQuantity)
        {
            var line = Lines.FirstOrDefault(l => l.SanPham.MaSanPham == maSanPham);
            if (line != null)
            {
                // Cập nhật số lượng sản phẩm
                line.SoLuong = newQuantity;
            }
        }

        public double ComputeTotalValues() =>
            (double)Lines.Sum(e => e.SanPham?.Gia * e.SoLuong );
        public void Clear() => Lines.Clear();

        public double TienGiam { get; set; }
        public double PhiVanChuyen { get; set; }

        public double TongTienThanhToan => ComputeTotalValues() + PhiVanChuyen - TienGiam;

        public int TotalWeightGram()
        {
            // Demo:  mỗi sản phẩm giả sử 500g
            return Lines.Sum(l => l.SoLuong * 500);
        }
    }


    public class GioHangLine
    {
        public int MaGioHangLine { get; set; }
        public SanPham SanPham { get; set; } = new();
        public int SoLuong { get; set; }
    }
}