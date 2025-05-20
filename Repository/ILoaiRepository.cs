using MyPhamSoul.Models;

namespace MyPhamSoul.Repository
{
    public interface ILoaiRepository
    {
        Loai Add(Loai loai);
        Loai Update(Loai loai);
        Loai Delete(string maloai);
        Loai GetLoai(string maloai);
        IEnumerable<Loai> GetAllLoai();



    }
    public interface IHangRepository
    {
        Hang Add(Hang hang);
        Hang Update(Hang hang);
        Hang Delete(string mahang);
        Hang GetHang(string mahang);
        IEnumerable<Hang> GetAllHang();


    }
    public interface ICTLoaiRepository
    {
        List<Ctloai> GetCtLoaiByLoai(string maLoai);
        Loai Add(Ctloai ctloai);
        Loai Update(Ctloai ctloai);
        Loai Delete(string mactloai);
        Loai GetLoai(string mactloai);
        IEnumerable<Ctloai> GetAllCTLoai();
    }


}
