using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ConstructionStore.Admin.Services;

public class ProductSummaryInOrder
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Description { get; set; }
}

public class OrderItemModel
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public ProductSummaryInOrder? Product { get; set; }
}

public class OrderModel
{
    public int Id { get; set; }
    public string CustomerFullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Address { get; set; } = string.Empty;
    public Guid OrderToken { get; set; }
    public DateTime OrderDate { get; set; }
    public string? Status { get; set; }
    public decimal? TotalPrice { get; set; }
    public string? Notes { get; set; }
    public List<OrderItemModel> OrderItems { get; set; } = new();
}

public class UpdateOrderStatusDto
{
    public string Status { get; set; } = string.Empty;
}

public class OrderService
{
    private readonly HttpClient _http;

    private readonly AuthStateService _auth;

    public OrderService(HttpClient http, AuthStateService auth)
    {
        _http = http;
        _auth = auth;
    }

    public async Task<List<OrderModel>> GetOrdersAsync()
    {
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, "api/admin/orders");
            if (_auth?.CurrentUser is not null && !string.IsNullOrEmpty(_auth.CurrentUser.Token))
            {
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _auth.CurrentUser.Token);
            }

            using var resp = await _http.SendAsync(req);
            if (!resp.IsSuccessStatusCode) return new List<OrderModel>();

            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<OrderModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
        }
        catch
        {
            return new();
        }
    }

    public async Task<bool> UpdateOrderStatusAsync(int id, string status)
    {
        try
        {
            var dto = new UpdateOrderStatusDto { Status = status };
            using var req = new HttpRequestMessage(HttpMethod.Put, $"api/admin/orders/{id}/status") { Content = JsonContent.Create(dto) };
            if (_auth?.CurrentUser is not null && !string.IsNullOrEmpty(_auth.CurrentUser.Token))
            {
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _auth.CurrentUser.Token);
            }

            using var resp = await _http.SendAsync(req);
            return resp.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<OrderModel?> GetOrderAsync(int id)
    {
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, $"api/admin/orders/{id}");
            if (_auth?.CurrentUser is not null && !string.IsNullOrEmpty(_auth.CurrentUser.Token))
            {
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _auth.CurrentUser.Token);
            }

            using var resp = await _http.SendAsync(req);
            if (!resp.IsSuccessStatusCode) return null;

            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<OrderModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            return null;
        }
    }
}
