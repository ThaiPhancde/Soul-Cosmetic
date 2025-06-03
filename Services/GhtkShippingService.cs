using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyPhamSoul.Services
{
    /// <summary>
    /// Triển khai IShippingService cho Giao Hàng Tiết Kiệm (GHTK)
    /// </summary>
    public class GhtkShippingService : IShippingService
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GhtkShippingService> _logger;

        public GhtkShippingService(HttpClient client, IConfiguration configuration, ILogger<GhtkShippingService> logger)
        {
            _client = client;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<decimal> CalculateFeeAsync(ShippingRateRequest req)
        {
            var payload = new
            {
                pick_province = "Hồ Chí Minh",
                pick_district = "Quận 1",
                province = "Hồ Chí Minh", 
                district = "Quận 1",
                weight = req.Weight,
                value = req.OrderValue,
                transport = "road" // road hoặc fly
            };

            try
            {
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                using var response = await _client.PostAsync("services/shipment/fee", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(responseJson);
                    var root = doc.RootElement;
                    
                    if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean())
                    {
                        if (root.TryGetProperty("fee", out var feeElement))
                        {
                            var fee = feeElement.GetDecimal();
                            return fee;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GHTK fee api error");
            }
            
            // Trả về phí mặc định nếu không tính được
            return 30000m;
        }

        public async Task<string> CreateShipmentAsync(ShipmentRequest req)
        {
            var payload = new
            {
                products = new[]
                {
                    new
                    {
                        name = "Mỹ phẩm",
                        weight = req.Weight / 1000.0, // GHTK tính theo kg
                        quantity = 1,
                        product_code = ""
                    }
                },
                order = new
                {
                    id = req.OrderCode,
                    pick_name = "Shop Mỹ Phẩm Soul",
                    pick_address = "123 Nguyễn Văn Cừ",
                    pick_province = "Hồ Chí Minh",
                    pick_district = "Quận 1",
                    pick_tel = "0901234567",
                    tel = req.ToPhone,
                    name = req.ToName,
                    address = req.ToAddress,
                    province = "Hồ Chí Minh",
                    district = "Quận 1",
                    is_freeship = 0,
                    pick_money = (int)req.OrderValue,
                    note = "Hàng dễ vỡ, vui lòng cẩn thận",
                    value = (int)req.OrderValue,
                    transport = "road"
                }
            };

            try
            {
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                _logger.LogInformation("Đang tạo đơn GHTK với payload: {Payload}", json);
                
                using var response = await _client.PostAsync("services/shipment/order", content);
                var responseJson = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("GHTK Response: {Response}", responseJson);
                
                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(responseJson);
                    var root = doc.RootElement;
                    
                    if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean())
                    {
                        if (root.TryGetProperty("order", out var orderElement) && 
                            orderElement.TryGetProperty("label", out var labelElement))
                        {
                            var orderCode = labelElement.GetString();
                            _logger.LogInformation("Tạo đơn GHTK thành công: {OrderCode}", orderCode);
                            return orderCode ?? string.Empty;
                        }
                    }
                    else
                    {
                        var message = root.TryGetProperty("message", out var msgElement) ? msgElement.GetString() : "Unknown error";
                        _logger.LogWarning("GHTK API error: {Message}", message);
                    }
                }
                else
                {
                    _logger.LogError("GHTK API trả về lỗi HTTP {StatusCode}: {Content}", response.StatusCode, responseJson);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GHTK create order error");
            }
            
            return string.Empty;
        }

        public async Task<ShipmentStatus> GetStatusAsync(string providerOrderCode)
        {
            try
            {
                using var response = await _client.GetAsync($"services/shipment/v2/{providerOrderCode}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    var root = doc.RootElement;
                    
                    if (root.TryGetProperty("success", out var successElement) && successElement.GetBoolean())
                    {
                        if (root.TryGetProperty("order", out var orderElement) && 
                            orderElement.TryGetProperty("status", out var statusElement))
                        {
                            var status = statusElement.GetInt32();
                            return MapGhtkStatus(status);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GHTK get status error");
            }
            
            return ShipmentStatus.Pending;
        }

        private static ShipmentStatus MapGhtkStatus(int ghtkStatus)
        {
            return ghtkStatus switch
            {
                1 => ShipmentStatus.Pending,      // Chưa tiếp nhận
                2 => ShipmentStatus.Confirmed,    // Đã tiếp nhận
                3 => ShipmentStatus.Confirmed,    // Đã lấy hàng
                4 => ShipmentStatus.Confirmed,    // Đã nhập kho
                5 => ShipmentStatus.Delivering,   // Đang giao hàng
                6 => ShipmentStatus.Delivered,    // Đã giao hàng
                7 => ShipmentStatus.Canceled,     // Đã hủy
                _ => ShipmentStatus.Pending,
            };
        }
    }
} 