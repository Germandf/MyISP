using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyISP.Bff;

public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGet("/my/services", async (HttpContext httpContext, [FromServices] ApplicationDbContext db, CancellationToken ct) =>
        {
            var customerId = Guid.Parse(httpContext.User.Claims.First(c => c.Type == "customerid").Value);

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
        })
        .WithName("GetMyServices")
        .RequireAuthorization();
    }
}
