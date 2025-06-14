using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyPhamSoul.Services
{
    /// <summary>
    /// Triển khai IShippingService cho đối tác Giao Hàng Nhanh (GHN)
    /// Tham khảo tài liệu: https://api.ghn.vn
    /// </summary>
    public class GhnShippingService : IShippingService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GhnShippingService> _logger;

        private readonly string _shopId;

        public GhnShippingService(HttpClient client, IConfiguration configuration, ILogger<GhnShippingService> logger)
        {
            _client = client;
            _configuration = configuration;
            _logger = logger;
            _shopId = configuration["Ghn:ShopId"] ?? string.Empty;
        }

        public async Task<decimal> CalculateFeeAsync(ShippingRateRequest req)
        {
            var payload = new
            {
                from_district_id = req.FromDistrictId,
                to_district_id = req.ToDistrictId,
                to_ward_code = req.ToWardCode,
                weight = req.Weight,
                insurance_value = req.OrderValue,
                service_id = 53320 // Gói dịch vụ cố định (có thể cần cấu hình)
            };

            try
            {
                using var response = await _client.PostAsJsonAsync("v2/shipping-order/fee", payload);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                var code = root.GetProperty("code").GetInt32();
                if (code == 200)
                {
                    var fee = root.GetProperty("data").GetProperty("total").GetDecimal();
                    return fee;
                }
                _logger.LogWarning("GHN fee api response code {code}, content: {content}", code, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GHN fee api error");
            }
            return 0m;
        }

        public async Task<string> CreateShipmentAsync(ShipmentRequest req)
        {
            // Validate input
            if (string.IsNullOrEmpty(_shopId) || !int.TryParse(_shopId, out var shopId))
            {
                _logger.LogError("Shop ID không hợp lệ: {ShopId}", _shopId);
                return string.Empty;
            }

            var payload = new
            {
                shop_id = shopId,
                from_district_id = req.FromDistrictId,
                to_district_id = req.ToDistrictId,
                to_ward_code = req.ToWardCode,
                to_name = req.ToName,
                to_phone = req.ToPhone,
                to_address = req.ToAddress,
                weight = req.Weight,
                insurance_value = (int)req.OrderValue,
                service_type_id = 2, // express
                payment_type_id = 1, // Người gửi trả phí ship (1) / người nhận (2)
                required_note = "CHOTHUHANG",
                cod_amount = (int)req.OrderValue,
                client_order_code = req.OrderCode,
                items = new[]
                {
                    new
                    {
                        name = "Mỹ phẩm",
                        quantity = 1,
                        weight = req.Weight
                    }
                }
            };

            try
            {
                _logger.LogInformation("Đang tạo đơn GHN với payload: {Payload}", JsonSerializer.Serialize(payload));
                
                using var response = await _client.PostAsJsonAsync("v2/shipping-order/create", payload);
                var json = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("GHN Response: {Response}", json);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("GHN API trả về lỗi HTTP {StatusCode}: {Content}", response.StatusCode, json);
                    return string.Empty;
                }
                
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                
                if (root.TryGetProperty("code", out var codeElement))
                {
                    var code = codeElement.GetInt32();
                    if (code == 200)
                    {
                        if (root.TryGetProperty("data", out var dataElement) && 
                            dataElement.TryGetProperty("order_code", out var orderCodeElement))
                        {
                            var orderCode = orderCodeElement.GetString();
                            _logger.LogInformation("Tạo đơn GHN thành công: {OrderCode}", orderCode);
                            return orderCode ?? string.Empty;
                        }
                    }
                    else
                    {
                        var message = root.TryGetProperty("message", out var msgElement) ? msgElement.GetString() : "Unknown error";
                        _logger.LogWarning("GHN API trả về mã lỗi {Code}: {Message}", code, message);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Lỗi kết nối đến GHN API");
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout khi gọi GHN API");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định khi tạo đơn GHN");
            }
            
            return string.Empty;
        }

        public async Task<ShipmentStatus> GetStatusAsync(string providerOrderCode)
        {
            var payload = new { order_code = providerOrderCode, shop_id = _shopId };

            try
            {
                using var response = await _client.PostAsJsonAsync("v2/shipping-order/detail", payload);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                var code = root.GetProperty("code").GetInt32();
                if (code == 200)
                {
                    var status = root.GetProperty("data").GetProperty("status").GetString();
                    return MapGhnStatus(status);
                }
                _logger.LogWarning("GHN get status response code {code}, content: {content}", code, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GHN get status error");
            }
            return ShipmentStatus.Pending;
        }

        private static ShipmentStatus MapGhnStatus(string? ghnStatus)
        {
            return ghnStatus switch
            {
                "ready_to_pick" => ShipmentStatus.Confirmed,
                "picked" => ShipmentStatus.Confirmed,
                "delivering" => ShipmentStatus.Delivering,
                "delivered" => ShipmentStatus.Delivered,
                "cancel" => ShipmentStatus.Canceled,
                _ => ShipmentStatus.Pending,
            };
        }
    }
} 