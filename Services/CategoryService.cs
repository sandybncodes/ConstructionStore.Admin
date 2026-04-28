using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ConstructionStore.Admin.Services;

public class CategoryModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CategoryService
{
    private readonly HttpClient _http;

    private readonly AuthStateService _auth;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public CategoryService(HttpClient http, AuthStateService auth)
    {
        _http = http;
        _auth = auth;
    }

    public async Task<List<CategoryModel>> GetCategoriesAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<CategoryModel>>("api/categories") ?? new();
        }
        catch
        {
            return new();
        }
    }

    public async Task<CategoryModel?> GetCategoryAsync(int id)
    {
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, $"api/admin/categories/{id}");
            AddAuthorizationHeader(req);

            using var resp = await _http.SendAsync(req);
            if (!resp.IsSuccessStatusCode) return null;

            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CategoryModel>(json, JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
    {
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Put, $"api/admin/categories/{id}")
            {
                Content = JsonContent.Create(dto)
            };

            AddAuthorizationHeader(req);

            using var resp = await _http.SendAsync(req);
            return resp.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<CategoryModel?> CreateCategoryAsync(CreateCategoryDto dto)
    {
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, "api/admin/categories")
            {
                Content = JsonContent.Create(dto)
            };

            AddAuthorizationHeader(req);

            using var resp = await _http.SendAsync(req);
            if (!resp.IsSuccessStatusCode) return null;

            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CategoryModel>(json, JsonOptions);
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
