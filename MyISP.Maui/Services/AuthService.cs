using System.Net.Http.Json;
using System.Text.Json;

namespace MyISP.Maui.Services;

public class AuthService(IHttpClientFactory httpClientFactory)
{
    public event EventHandler? LoggedOut;

    public async Task<string?> LoginAsync(string userOrEmail, string password, CancellationToken ct = default)
    {
        var client = httpClientFactory.CreateClient("Identity");

        var payload = new
        {
            email = userOrEmail,
            password = password,
            useCookies = false,
            useSessionCookies = false
        };

        using var resp = await client.PostAsJsonAsync("/login", payload, ct);
        if (!resp.IsSuccessStatusCode)
            return null;

        var json = await resp.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(json);
        if (!doc.RootElement.TryGetProperty("accessToken", out var tokenProp))
            return null;

        var token = tokenProp.GetString();
        if (string.IsNullOrWhiteSpace(token))
            return null;

        await SecureStorage.SetAsync("auth_token", token);
        return token;
    }

    public async Task<bool> RegisterAsync(string email, string password, CancellationToken ct = default)
    {
        var client = httpClientFactory.CreateClient("Identity");
        var payload = new { email, password };
        using var resp = await client.PostAsJsonAsync("/register", payload, ct);
        return resp.IsSuccessStatusCode;
    }

    public async Task<string?> GetTokenAsync()
    {
        return await SecureStorage.GetAsync("auth_token");
    }

    public Task LogoutAsync()
    {
        SecureStorage.Remove("auth_token");
        LoggedOut?.Invoke(this, EventArgs.Empty);
        return Task.CompletedTask;
    }

    public async Task<bool> IsTokenValidAsync(string token, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(token)) return false;
        try
        {
            var client = httpClientFactory.CreateClient("Identity");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            using var resp = await client.GetAsync("/me", ct);
            if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return false;
            return resp.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
