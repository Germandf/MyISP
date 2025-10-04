using System.Net.Http.Json;
using System.Text.Json;

namespace MyISP.Maui.Services;

public class AuthService(IHttpClientFactory httpClientFactory)
{
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

    public async Task<string?> GetTokenAsync()
    {
        return await SecureStorage.GetAsync("auth_token");
    }

    public Task LogoutAsync()
    {
        SecureStorage.Remove("auth_token");
        return Task.CompletedTask;
    }
}
