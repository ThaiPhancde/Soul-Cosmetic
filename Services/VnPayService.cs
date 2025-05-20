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
                var timeNow = DateTime.Now;
                var tick = timeNow.Ticks.ToString();
                var vnpay = new VnPayLibrary();
                
                vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"]);
                vnpay.AddRequestData("vnp_Command", _config["VnPay:Command"]);
                vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);
                
                // Đảm bảo format số tiền chính xác (số nguyên, không có thập phân)
                long amountInVnd = Convert.ToInt64(Math.Floor(model.Amount * 100));
                vnpay.AddRequestData("vnp_Amount", amountInVnd.ToString());
                
                // Định dạng ngày giờ chính xác theo yêu cầu của VNPAY
                string createDate = timeNow.ToString("yyyyMMddHHmmss");
                vnpay.AddRequestData("vnp_CreateDate", createDate);
                vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"]);
                
                // Lấy IP chính xác của khách hàng
                string ipAddress = Utils.GetIpAddress(context);
                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = "127.0.0.1"; // IP mặc định nếu không lấy được
                }
                vnpay.AddRequestData("vnp_IpAddr", ipAddress);
                
                vnpay.AddRequestData("vnp_Locale", _config["VnPay:Locale"]);
                
                // Loại bỏ dấu tiếng Việt và ký tự đặc biệt trong OrderInfo
                string orderInfo = "Thanh toan don hang " + model.OrderId;
                vnpay.AddRequestData("vnp_OrderInfo", orderInfo);
                
                vnpay.AddRequestData("vnp_OrderType", "other");
                vnpay.AddRequestData("vnp_ReturnUrl", _config["VnPay:PaymentBackReturnUrl"]);
                
                // Sử dụng mã giao dịch duy nhất, đảm bảo không có ký tự đặc biệt
                string txnRef = model.OrderId;
                // Đảm bảo txnRef không có ký tự đặc biệt và khoảng trắng
                txnRef = System.Text.RegularExpressions.Regex.Replace(txnRef, @"[^a-zA-Z0-9]", string.Empty);
                vnpay.AddRequestData("vnp_TxnRef", txnRef);
                
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
                // Log lỗi
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
