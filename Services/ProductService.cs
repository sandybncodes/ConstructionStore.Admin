using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ConstructionStore.Admin.Services;

public class ProductImage
{
    public int ProductId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsMain { get; set; }
}

public class ProductModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Discount { get; set; }
    public int StockQuantity { get; set; }
    public int? CategoryId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public object? Category { get; set; }
    public List<ProductImage> Images { get; set; } = new();
}

public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Discount { get; set; }
    public int StockQuantity { get; set; }
    public int? CategoryId { get; set; }
    public bool IsActive { get; set; }
}

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Discount { get; set; }
    public int StockQuantity { get; set; }
    public int? CategoryId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class ProductService
{
    private readonly HttpClient _http;
    private readonly AuthStateService _auth;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ProductService(HttpClient http, AuthStateService auth)
    {
        _http = http;
        _auth = auth;
    }

    public async Task<List<ProductModel>> GetProductsAsync()
    {
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, "api/admin/products");
            AddAuthorizationHeader(req);

            using var resp = await _http.SendAsync(req);
            if (!resp.IsSuccessStatusCode) return new List<ProductModel>();

            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ProductModel>>(json, JsonOptions) ?? new();
        }
        catch
        {
            return new();
        }
    }

    public async Task<ProductModel?> GetProductAsync(int id)
    {
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, $"api/admin/products/{id}");
            AddAuthorizationHeader(req);

            using var resp = await _http.SendAsync(req);
            if (!resp.IsSuccessStatusCode) return null;

            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ProductModel>(json, JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    public async Task<ProductModel?> UpdateProductAsync(int id, UpdateProductDto dto)
    {
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Put, $"api/admin/products/{id}")
            {
                Content = JsonContent.Create(dto)
            };

            AddAuthorizationHeader(req);

            using var resp = await _http.SendAsync(req);
            if (!resp.IsSuccessStatusCode) return null;

            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ProductModel>(json, JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    public async Task<ProductModel?> CreateProductAsync(CreateProductDto dto)
    {
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, "api/admin/products")
            {
                Content = JsonContent.Create(dto)
            };

            AddAuthorizationHeader(req);

            using var resp = await _http.SendAsync(req);
            if (!resp.IsSuccessStatusCode) return null;

            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ProductModel>(json, JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    private void AddAuthorizationHeader(HttpRequestMessage request)
    {
        if (_auth?.CurrentUser is not null && !string.IsNullOrEmpty(_auth.CurrentUser.Token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _auth.CurrentUser.Token);
        }
    }
}
