using System.Net.Http.Json;
using Microsoft.Maui.Storage;

namespace ConstructionStore.Admin.Services;

public class AdminUserInfo
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string TokenType { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class AuthStateService
{
    private readonly HttpClient _httpClient;
    private readonly LocalizationService _localization;
    private const string TokenKey = "cs_auth_token";
    private const string FullNameKey = "cs_auth_fullname";
    private const string EmailKey = "cs_auth_email";
    private const string RoleKey = "cs_auth_role";

    public AdminUserInfo? CurrentUser { get; private set; }
    public bool IsAuthenticated => CurrentUser != null;
    public bool WasJustLoggedIn { get; private set; }

    public event Action? AuthStateChanged;

    public AuthStateService(HttpClient httpClient, LocalizationService localization)
    {
        _httpClient = httpClient;
        _localization = localization;
    }

    public async Task InitializeAsync()
    {
        try
        {
            var token = await SecureStorage.GetAsync(TokenKey);
            if (!string.IsNullOrEmpty(token))
            {
                CurrentUser = new AdminUserInfo
                {
                    Token = token,
                    FullName = await SecureStorage.GetAsync(FullNameKey) ?? string.Empty,
                    Email = await SecureStorage.GetAsync(EmailKey) ?? string.Empty,
                    Role = await SecureStorage.GetAsync(RoleKey) ?? string.Empty,
                };
                WasJustLoggedIn = false;
            }
        }
        catch
        {
            // SecureStorage may not be available on all platforms during development
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> LoginAsync(string email, string password)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/admin/login", new { email, password });

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result is not null)
                {
                    CurrentUser = new AdminUserInfo
                    {
                        Token = result.Token,
                        FullName = result.FullName,
                        Email = result.Email,
                        Role = result.Role,
                    };

                    try
                    {
                        await SecureStorage.SetAsync(TokenKey, result.Token);
                        await SecureStorage.SetAsync(FullNameKey, result.FullName);
                        await SecureStorage.SetAsync(EmailKey, result.Email);
                        await SecureStorage.SetAsync(RoleKey, result.Role);
                    }
                    catch { /* ignore if SecureStorage unavailable */ }

                    WasJustLoggedIn = true;
                    AuthStateChanged?.Invoke();
                    return (true, null);
                }
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return (false, _localization.T("errors.invalidCredentials"));

            var errorContent = await response.Content.ReadAsStringAsync();
            return (false, string.IsNullOrEmpty(errorContent) ? _localization.T("errors.loginFailed") : errorContent.Trim('"'));
        }
        catch (HttpRequestException)
        {
            return (false, _localization.T("errors.connection"));
        }
        catch
        {
            return (false, _localization.T("errors.unexpected"));
        }
    }

    public Task LogoutAsync()
    {
        CurrentUser = null;
        WasJustLoggedIn = false;

        try
        {
            SecureStorage.Remove(TokenKey);
            SecureStorage.Remove(FullNameKey);
            SecureStorage.Remove(EmailKey);
            SecureStorage.Remove(RoleKey);
        }
        catch { /* ignore */ }

        AuthStateChanged?.Invoke();
        return Task.CompletedTask;
    }

    public void MarkLoginNotificationHandled()
    {
        WasJustLoggedIn = false;
    }
}
