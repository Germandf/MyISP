using System.Net.Http.Headers;
using System.Net.Http.Json;
using MyISP.Maui.Models;

namespace MyISP.Maui.Services;

public class UserServicesService(IHttpClientFactory httpClientFactory, AuthService authService)
{
    public async Task<MyServicesResponse?> GetMyServicesAsync(CancellationToken ct = default)
    {
        var token = await authService.GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var client = httpClientFactory.CreateClient("Bff");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var resp = await client.GetAsync("/my/services", ct);
        if (!resp.IsSuccessStatusCode)
            return null;

        return await resp.Content.ReadFromJsonAsync<MyServicesResponse>(cancellationToken: ct);
    }
}
