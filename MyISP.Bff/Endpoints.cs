using Microsoft.EntityFrameworkCore;

namespace MyISP.Bff;

public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGet("/my/services", async (ApplicationDbContext db, CancellationToken ct) =>
        {
            var customerId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            var internet = await db.InternetServiceSubscriptions
                .Where(s => s.CustomerId == customerId && s.Status == SubscriptionStatus.Active)
                .Include(s => s.InternetService)
                .Select(s => new
                {
                    s.Id,
                    Service = new { s.InternetService.Id, s.InternetService.Name, s.InternetService.Mbps, s.InternetService.MonthlyPrice }
                })
                .ToListAsync(ct);

            var mobile = await db.MobileServiceSubscriptions
                .Where(s => s.CustomerId == customerId && s.Status == SubscriptionStatus.Active)
                .Include(s => s.MobileService)
                .Select(s => new
                {
                    s.Id,
                    Service = new { s.MobileService.Id, s.MobileService.Name, s.MobileService.Gbs, s.MobileService.MonthlyPrice }
                })
                .ToListAsync(ct);

            return Results.Ok(new { internet, mobile });
        }).RequireAuthorization();
    }
}
