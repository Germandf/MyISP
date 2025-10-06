using MyISP.Maui.Models;
using System.Net.Http.Json;

namespace MyISP.Maui.Services;

public class InvoicesService(IHttpClientFactory httpClientFactory, AuthService authService)
{
    public async Task<MyInvoicesResponse?> GetMyInvoicesAsync(int months = 12, CancellationToken ct = default)
    {
        var token = await authService.GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var client = httpClientFactory.CreateClient("Bff");
        using var resp = await client.GetAsync($"/my/invoices?months={months}", ct);
        if (!resp.IsSuccessStatusCode)
            return null;

        return await resp.Content.ReadFromJsonAsync<MyInvoicesResponse>(cancellationToken: ct);
    }
}
