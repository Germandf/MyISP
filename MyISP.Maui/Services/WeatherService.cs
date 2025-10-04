using MyISP.Maui.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MyISP.Maui.Services;

public class WeatherService(IHttpClientFactory httpClientFactory, AuthService authService)
{
    public async Task<IReadOnlyList<WeatherForecast>?> GetForecastAsync(CancellationToken ct = default)
    {
        var token = await authService.GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
            return null;

        var client = httpClientFactory.CreateClient("Bff");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var resp = await client.GetAsync("/weatherforecast", ct);
        if (!resp.IsSuccessStatusCode)
            return null;

        var forecasts = await resp.Content.ReadFromJsonAsync<List<WeatherForecast>>(cancellationToken: ct);
        return forecasts;
    }
}
