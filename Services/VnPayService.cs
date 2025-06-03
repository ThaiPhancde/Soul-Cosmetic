using MyPhamSoul.Helpper;
using MyPhamSoul.ModelViews;
using System.Text;
using System.Globalization;

namespace MyPhamSoul.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<VnPayService> _logger;
        
        public VnPayService(IConfiguration config, ILogger<VnPayService> logger)
        {
            _config = config;
            _logger = logger;
        }
        
        public string GetVnPayConfig(string key)
        {
            return _config[$"VnPay:{key}"];
        }
        
        public string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model)
        {
            try
            {
                var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
                var vnpay = new VnPayLibrary();
                
                vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"]);
                vnpay.AddRequestData("vnp_Command", _config["VnPay:Command"]);
                vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);
                
                // Số tiền * 100 để loại bỏ phần thập phân
                long amountInVnd = Convert.ToInt64(Math.Round(model.Amount) * 100);
                vnpay.AddRequestData("vnp_Amount", amountInVnd.ToString());
                
                vnpay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"]);
                
                // Lấy IP người dùng
                string ipAddress = Utils.GetIpAddress(context);
                if (string.IsNullOrEmpty(ipAddress) || ipAddress.Contains("Invalid"))
                {
                    ipAddress = "192.168.10.205"; // IP mặc định nếu không lấy được
                }
                vnpay.AddRequestData("vnp_IpAddr", ipAddress);
                
                vnpay.AddRequestData("vnp_Locale", _config["VnPay:Locale"]);
                vnpay.AddRequestData("vnp_OrderInfo", model.Description);
                vnpay.AddRequestData("vnp_OrderType", "other"); // Mã danh mục hàng hóa
                vnpay.AddRequestData("vnp_ReturnUrl", _config["VnPay:PaymentBackReturnUrl"]);
                
                // Thêm mã đơn hàng - phải là duy nhất trong ngày
                vnpay.AddRequestData("vnp_TxnRef", model.OrderId);
                
                // Thêm thời gian hết hạn thanh toán
                var expireDate = timeNow.AddMinutes(15);
                vnpay.AddRequestData("vnp_ExpireDate", expireDate.ToString("yyyyMMddHHmmss"));
                
                // Thêm thông tin khách hàng nếu có
                if (!string.IsNullOrEmpty(model.CustomerId))
                {
                    vnpay.AddRequestData("vnp_CustomerId", model.CustomerId);
                }

                // Tạo URL thanh toán và log để debug
                var paymentUrl = vnpay.CreateRequestUrl(_config["VnPay:BaseUrl"], _config["VnPay:HashSecret"]);
                _logger.LogInformation("VNPAY URL: " + paymentUrl);
                
                return paymentUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError("VNPay Error: " + ex.Message);
                throw;
            }
        }

        public VnPaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            try
            {
                var vnpay = new VnPayLibrary();
                // Log tất cả các tham số nhận được từ VNPay để debug
                _logger.LogInformation("VNPay Response parameters:");
                foreach (var (key, value) in collections)
                {
                    _logger.LogInformation($"{key}: {value}");
                    if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(key, value.ToString());
                    }
                }
                
                var vnp_orderId = vnpay.GetResponseData("vnp_TxnRef");
                var vnp_TransactionId = vnpay.GetResponseData("vnp_TransactionNo");
                var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
                var vnp_Amount = vnpay.GetResponseData("vnp_Amount");
                var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;

                double amount = 0;
                if (!string.IsNullOrEmpty(vnp_Amount))
                {
                    if (double.TryParse(vnp_Amount, out double parsedAmount))
                    {
                        amount = parsedAmount / 100; // Chuyển đổi giá trị về đơn vị tiền tệ thực tế
                    }
                    else
                    {
                        _logger.LogError($"Không thể chuyển đổi vnp_Amount: {vnp_Amount} thành số");
                    }
                }

                // Log thông tin xác thực để debug
                _logger.LogInformation($"VNPay Response: OrderId={vnp_orderId}, ResponseCode={vnp_ResponseCode}, Amount={amount}");
                
                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config["VnPay:HashSecret"]);
                if (!checkSignature)
                {
                    _logger.LogError("VNPay Error: Invalid signature");
                    return new VnPaymentResponseModel
                    {
                        Success = false,
                        VnPayResponseCode = "97", // Mã lỗi chữ ký không hợp lệ
                        OrderId = vnp_orderId
                    };
                }

                return new VnPaymentResponseModel
                {
                    Success = vnp_ResponseCode == "00",
                    PaymentMethod = "VnPay",
                    OrderDescription = vnp_OrderInfo,
                    OrderId = vnp_orderId,
                    TransactionId = vnp_TransactionId,
                    Token = vnp_SecureHash,
                    VnPayResponseCode = vnp_ResponseCode,
                    Amount = amount
                };
            }
            catch (Exception ex)
            {
                // Log lỗi
                _logger.LogError($"VNPay Response Error: {ex.Message}");
                return new VnPaymentResponseModel
                {
                    Success = false,
                    VnPayResponseCode = "99", // Mã lỗi không xác định
                    OrderDescription = ex.Message
                };
            }
        }
    }
}
