using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Forms;

namespace ConstructionStore.Admin.Services;

public class ProductImage
{
    private string? _imageUrl;

    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ImageUrl
    {
        get => _imageUrl ?? string.Empty;
        set => _imageUrl = value;
    }

    public bool IsMain { get; set; }
}

public sealed class ProductImageUploadResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public IReadOnlyList<ProductImage> Images { get; init; } = Array.Empty<ProductImage>();
}

public sealed class ProductImageDeleteResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public ProductModel? Product { get; init; }
}

public class ProductModel
{
    private List<ProductImage>? _images;
    private List<ProductVariantModel>? _variants;

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public CategoryModel? Category { get; set; }
    public List<ProductImage> Images
    {
        get => _images ??= new List<ProductImage>();
        set => _images = value ?? new List<ProductImage>();
    }
    public List<ProductVariantModel> Variants
    {
        get => _variants ??= new List<ProductVariantModel>();
        set => _variants = value ?? new List<ProductVariantModel>();
    }
}

public class AttributeModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class VariantAttributeValueModel
{
    public int AttributeId { get; set; }
    public string AttributeName { get; set; } = string.Empty;
    public string? ValueText { get; set; }
    public decimal? ValueNumeric { get; set; }
    public string? Unit { get; set; }
}

public class ProductVariantModel
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? Sku { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<VariantAttributeValueModel> Attributes { get; set; } = new();
}

public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
    public bool IsActive { get; set; }
}

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
    public bool IsActive { get; set; } = true;
    public List<CreateProductVariantDto> Variants { get; set; } = new();
}

public class CreateProductVariantDto
{
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? Sku { get; set; }
    public bool IsActive { get; set; } = true;
    public List<CreateVariantAttributeDto> Attributes { get; set; } = new();
}

public class CreateVariantAttributeDto
{
    public int AttributeId { get; set; }
    public string? ValueText { get; set; }
    public decimal? ValueNumeric { get; set; }
    public string? Unit { get; set; }
}

public class UpdateVariantAttributeDto
{
    public int AttributeId { get; set; }
    public string? ValueText { get; set; }
    public decimal? ValueNumeric { get; set; }
    public string? Unit { get; set; }
}

public class UpdateProductVariantDto
{
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? Sku { get; set; }
    public bool IsActive { get; set; }
    public List<UpdateVariantAttributeDto> Attributes { get; set; } = new();
}

public class ProductService
{
    private readonly HttpClient _http;
    private readonly AuthStateService _auth;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private const long MaxUploadSizeBytes = 10 * 1024 * 1024;

    public ProductService(HttpClient http, AuthStateService auth)
    {
        _http = http;
        _auth = auth;
    }

    public async Task<List<AttributeModel>> GetAttributesAsync()
    {
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, "api/admin/attributes");
            AddAuthorizationHeader(req);

            using var resp = await _http.SendAsync(req);
            if (!resp.IsSuccessStatusCode) return new List<AttributeModel>();

            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<AttributeModel>>(json, JsonOptions) ?? new();
        }
        catch
        {
            return new();
        }
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

    public async Task<ProductImageUploadResult> UploadProductImagesAsync(int productId, IReadOnlyList<IBrowserFile> files)
    {
        if (files.Count == 0)
        {
            return new ProductImageUploadResult { Success = true };
        }

        try
        {
            using var content = new MultipartFormDataContent();

            foreach (var file in files)
            {
                var stream = file.OpenReadStream(MaxUploadSizeBytes);
                var streamContent = new StreamContent(stream);
                streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(ResolveImageContentType(file.Name, file.ContentType));
                content.Add(streamContent, "files", file.Name);
            }

            using var req = new HttpRequestMessage(HttpMethod.Post, $"api/admin/products/{productId}/images")
            {
                Content = content
            };

            AddAuthorizationHeader(req);

            using var resp = await _http.SendAsync(req);
            if (!resp.IsSuccessStatusCode)
            {
                return new ProductImageUploadResult
                {
                    Success = false,
                    ErrorMessage = await resp.Content.ReadAsStringAsync()
                };
            }

            var json = await resp.Content.ReadAsStringAsync();
            var images = JsonSerializer.Deserialize<List<ProductImage>>(json, JsonOptions) ?? new List<ProductImage>();
            return new ProductImageUploadResult
            {
                Success = true,
                Images = images
            };
        }
        catch (Exception exception)
        {
            return new ProductImageUploadResult
            {
                Success = false,
                ErrorMessage = exception.Message
            };
        }
    }

    public async Task<ProductImageDeleteResult> DeleteProductImageAsync(int productId, int imageId)
    {
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Delete, $"api/admin/products/{productId}/images/{imageId}");

            AddAuthorizationHeader(req);

            using var resp = await _http.SendAsync(req);
            if (!resp.IsSuccessStatusCode)
            {
                return new ProductImageDeleteResult
                {
                    Success = false,
                    ErrorMessage = await resp.Content.ReadAsStringAsync()
                };
            }

            var json = await resp.Content.ReadAsStringAsync();
            return new ProductImageDeleteResult
            {
                Success = true,
                Product = JsonSerializer.Deserialize<ProductModel>(json, JsonOptions)
            };
        }
        catch (Exception exception)
        {
            return new ProductImageDeleteResult
            {
                Success = false,
                ErrorMessage = exception.Message
            };
        }
    }

    public async Task<ProductImageUploadResult> UploadProductImagesAsync(int productId, IReadOnlyList<FileResult> files)
    {
        if (files.Count == 0)
        {
            return new ProductImageUploadResult { Success = true };
        }

        try
        {
            using var content = new MultipartFormDataContent();

            foreach (var file in files)
            {
                var stream = await file.OpenReadAsync();
                var streamContent = new StreamContent(stream);
                var contentType = ResolveImageContentType(file.FileName, null);
                streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
                content.Add(streamContent, "files", file.FileName);
            }

            using var req = new HttpRequestMessage(HttpMethod.Post, $"api/admin/products/{productId}/images")
            {
                Content = content
            };

            AddAuthorizationHeader(req);

            using var resp = await _http.SendAsync(req);
            if (!resp.IsSuccessStatusCode)
            {
                return new ProductImageUploadResult
                {
                    Success = false,
                    ErrorMessage = await resp.Content.ReadAsStringAsync()
                };
            }

            var json = await resp.Content.ReadAsStringAsync();
            var images = JsonSerializer.Deserialize<List<ProductImage>>(json, JsonOptions) ?? new List<ProductImage>();
            return new ProductImageUploadResult
            {
                Success = true,
                Images = images
            };
        }
        catch (Exception exception)
        {
            return new ProductImageUploadResult
            {
                Success = false,
                ErrorMessage = exception.Message
            };
        }
    }

    public async Task<ProductModel?> UpdateVariantAsync(int productId, int variantId, UpdateProductVariantDto dto)
    {
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Put, $"api/admin/products/{productId}/variants/{variantId}")
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

    public async Task<ProductModel?> CreateVariantAsync(int productId, CreateProductVariantDto dto)
    {
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, $"api/admin/products/{productId}/variants")
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

    public async Task<(bool Success, ProductModel? Product)> DeleteVariantAsync(int productId, int variantId)
    {
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Delete, $"api/admin/products/{productId}/variants/{variantId}");
            AddAuthorizationHeader(req);
            using var resp = await _http.SendAsync(req);
            if (!resp.IsSuccessStatusCode) return (false, null);
            var json = await resp.Content.ReadAsStringAsync();
            return (true, JsonSerializer.Deserialize<ProductModel>(json, JsonOptions));
        }
        catch
        {
            return (false, null);
        }
    }

    private void AddAuthorizationHeader(HttpRequestMessage request)
    {
        if (_auth?.CurrentUser is not null && !string.IsNullOrEmpty(_auth.CurrentUser.Token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _auth.CurrentUser.Token);
        }
    }

    private static string ResolveImageContentType(string fileName, string? providedContentType)
    {
        if (!string.IsNullOrWhiteSpace(providedContentType))
        {
            return providedContentType;
        }

        return Path.GetExtension(fileName).ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            ".gif" => "image/gif",
            ".heic" => "image/heic",
            ".heif" => "image/heif",
            _ => "application/octet-stream"
        };
    }
}
