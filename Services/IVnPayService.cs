using MyPhamSoul.ModelViews;

namespace MyPhamSoul.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
         VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
        string GetVnPayConfig(string key);
    }
}
