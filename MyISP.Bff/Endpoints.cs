using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyISP.Bff;

public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGet("/my/services", async (
            HttpContext httpContext,
            [FromServices] ApplicationDbContext db,
            CancellationToken ct) =>
        {
            var customerId = Guid.Parse(httpContext.User.Claims.First(c => c.Type == "customerid").Value);

            var internet = await db.InternetServiceSubscriptions.AsNoTracking()
                .Where(s => s.CustomerId == customerId && s.Status == SubscriptionStatus.Active)
                .Include(s => s.InternetService)
                .Select(s => new
                {
                    s.Id,
                    s.Status,
                    s.StartDate,
                    Service = new { s.InternetService.Id, s.InternetService.Name, s.InternetService.Mbps, s.InternetService.MonthlyPrice }
                })
                .ToListAsync(ct);

            var mobile = await db.MobileServiceSubscriptions.AsNoTracking()
                .Where(s => s.CustomerId == customerId && s.Status == SubscriptionStatus.Active)
                .Include(s => s.MobileService)
                .Select(s => new
                {
                    s.Id,
                    s.Status,
                    s.StartDate,
                    Service = new { s.MobileService.Id, s.MobileService.Name, s.MobileService.Gbs, s.MobileService.MonthlyPrice }
                })
                .ToListAsync(ct);

            return Results.Ok(new { internet, mobile });
        })
        .WithName("GetMyServices")
        .RequireAuthorization();

        app.MapGet("/my/invoices", async (
            HttpContext httpContext,
            [FromServices] ApplicationDbContext db,
            [FromQuery] int months,
            CancellationToken ct) =>
        {
            months = months <= 0 ? 12 : months;
            var cutoff = DateTimeOffset.UtcNow.AddMonths(-months);
            var customerId = Guid.Parse(httpContext.User.Claims.First(c => c.Type == "customerid").Value);

            var query = db.Invoices.AsNoTracking()
                .Where(i => i.CustomerId == customerId && i.IssueDate >= cutoff)
                .OrderByDescending(i => i.IssueDate)
                .Select(i => new
                {
                    i.Id,
                    i.InvoiceNumber,
                    i.Status,
                    i.IssueDate,
                    i.DueDate,
                    i.TotalAmount
                });

            var list = await query.ToListAsync(ct);
            var pending = list.Where(i => i.Status == InvoiceStatus.Pending).ToList();
            var paid = list.Where(i => i.Status == InvoiceStatus.Paid).ToList();

            return Results.Ok(new { pending, paid });
        })
        .WithName("GetMyInvoices")
        .RequireAuthorization();

        app.MapGet("/my/contact-info", async (
            HttpContext httpContext,
            [FromServices] ApplicationDbContext db,
            CancellationToken ct) =>
        {
            var customerId = Guid.Parse(httpContext.User.Claims.First(c => c.Type == "customerid").Value);

            var emails = await db.CustomerEmails.AsNoTracking()
                .Where(e => e.CustomerId == customerId)
                .OrderByDescending(e => e.IsVerified)
                .Select(e => new { e.Id, e.Address, e.IsVerified, e.Label })
                .ToListAsync(ct);

            var phones = await db.CustomerPhones.AsNoTracking()
                .Where(p => p.CustomerId == customerId)
                .OrderByDescending(p => p.IsVerified)
                .Select(p => new { p.Id, p.AreaCode, p.LocalNumber, p.IsVerified, p.Label })
                .ToListAsync(ct);

            return Results.Ok(new { emails, phones });
        })
        .WithName("GetMyContactInfo")
        .RequireAuthorization();

        app.MapGet("/my/technical-requests", async (
            HttpContext httpContext,
            [FromServices] ApplicationDbContext db,
            CancellationToken ct) =>
        {
            var customerId = Guid.Parse(httpContext.User.Claims.First(c => c.Type == "customerid").Value);

            var items = await db.TechnicalRequests.AsNoTracking()
                .Where(t => t.CustomerId == customerId)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new
                {
                    t.Id,
                    t.TicketNumber,
                    t.Status,
                    t.Priority,
                    t.Subject,
                    t.Description,
                    t.CreatedAt,
                    t.ResolvedAt
                })
                .ToListAsync(ct);

            return Results.Ok(items);
        })
        .WithName("GetMyTechnicalRequests")
        .RequireAuthorization();
    }
}
