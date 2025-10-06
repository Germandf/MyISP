using MyISP.Maui.Models;
using System.Net.Http.Json;

namespace MyISP.Maui.Services;

public class ContactInfoService(IHttpClientFactory httpClientFactory, AuthService authService)
{
    public async Task<MyContactInfoResponse?> GetMyContactInfoAsync(CancellationToken ct = default)
    {
        var token = await authService.GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var client = httpClientFactory.CreateClient("Bff");
        using var resp = await client.GetAsync("/my/contact-info", ct);
        if (!resp.IsSuccessStatusCode)
            return null;

        return await resp.Content.ReadFromJsonAsync<MyContactInfoResponse>(cancellationToken: ct);
    }
}
