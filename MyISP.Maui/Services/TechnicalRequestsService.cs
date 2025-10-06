using MyISP.Maui.Models;
using System.Net.Http.Json;

namespace MyISP.Maui.Services;

public class TechnicalRequestsService(IHttpClientFactory httpClientFactory, AuthService authService)
{
    public async Task<List<TechnicalRequestDto>?> GetMyTechnicalRequestsAsync(CancellationToken ct = default)
    {
        var token = await authService.GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var client = httpClientFactory.CreateClient("Bff");
        using var resp = await client.GetAsync("/my/technical-requests", ct);
        if (!resp.IsSuccessStatusCode)
            return null;

        return await resp.Content.ReadFromJsonAsync<List<TechnicalRequestDto>>(cancellationToken: ct);
    }
}
