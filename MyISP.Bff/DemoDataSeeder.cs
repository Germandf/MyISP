using Bogus;
using Microsoft.EntityFrameworkCore;

namespace MyISP.Bff;

public static class DemoDataSeeder
{
    public static async Task SeedDemoAsync(this WebApplication app, CancellationToken ct = default)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.EnsureCreatedAsync(ct);

        Randomizer.Seed = new Random(42);
        var identityCustomerId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        if (!await db.Customers.AnyAsync(ct))
        {
            var customer = new Customer
            {
                Id = identityCustomerId,
                DisplayName = "Demo Customer",
                CreatedAt = DateTimeOffset.UtcNow.AddMonths(-18)
            };
            db.Customers.Add(customer);
            await db.SaveChangesAsync(ct);
        }

        if (!await db.InternetServices.AnyAsync(ct))
        {
            var internetCatalog = new[]
            {
                new InternetService { Name = "Fiber 300 Mbps", Mbps = 300, MonthlyPrice = 29.99m },
                new InternetService { Name = "Fiber 600 Mbps", Mbps = 600, MonthlyPrice = 39.99m },
                new InternetService { Name = "Fiber 1 Gbps",   Mbps = 1000, MonthlyPrice = 49.99m },
            };
            db.InternetServices.AddRange(internetCatalog);
            await db.SaveChangesAsync(ct);
        }

        if (!await db.MobileServices.AnyAsync(ct))
        {
            var mobileCatalog = new[]
            {
                new MobileService { Name = "Mobile 10 GB",  Gbs = 10,  MonthlyPrice = 9.99m },
                new MobileService { Name = "Mobile 30 GB",  Gbs = 30,  MonthlyPrice = 14.99m },
                new MobileService { Name = "Mobile 100 GB", Gbs = 100, MonthlyPrice = 24.99m },
            };
            db.MobileServices.AddRange(mobileCatalog);
            await db.SaveChangesAsync(ct);
        }

        if (!await db.InternetServiceSubscriptions.AnyAsync(ct))
        {
            var firstInternetPlan = await db.InternetServices.OrderBy(s => s.MonthlyPrice).FirstAsync(ct);
            db.InternetServiceSubscriptions.Add(new InternetServiceSubscription
            {
                CustomerId = identityCustomerId,
                InternetServiceId = firstInternetPlan.Id,
                Status = SubscriptionStatus.Active,
                StartDate = DateTimeOffset.UtcNow.AddMonths(-18)
            });
            await db.SaveChangesAsync(ct);
        }

        if (!await db.MobileServiceSubscriptions.AnyAsync(ct))
        {
            var firstMobilePlan = await db.MobileServices.OrderBy(s => s.MonthlyPrice).FirstAsync(ct);
            db.MobileServiceSubscriptions.Add(new MobileServiceSubscription
            {
                CustomerId = identityCustomerId,
                MobileServiceId = firstMobilePlan.Id,
                Status = SubscriptionStatus.Active,
                StartDate = DateTimeOffset.UtcNow.AddMonths(-18)
            });
            await db.SaveChangesAsync(ct);
        }

        if (!await db.CustomerEmails.AnyAsync(ct))
        {
            var emailFaker = new Faker<CustomerEmail>()
                .RuleFor(x => x.CustomerId, _ => identityCustomerId)
                .RuleFor(x => x.Address, f => f.Internet.Email("demo.customer"))
                .RuleFor(x => x.IsVerified, f => f.Random.Bool(0.8f))
                .RuleFor(x => x.Label, f => f.PickRandom(new[] { "Primary", "Billing", "Other", null }));
            db.CustomerEmails.AddRange(emailFaker.Generate(2));
            await db.SaveChangesAsync(ct);
        }

        if (!await db.CustomerPhones.AnyAsync(p => p.CustomerId == identityCustomerId, ct))
        {
            var phoneFaker = new Faker<CustomerPhone>()
                .RuleFor(x => x.CustomerId, _ => identityCustomerId)
                .RuleFor(x => x.AreaCode, f => f.Random.Int(200, 999).ToString())
                .RuleFor(x => x.LocalNumber, f => f.Phone.PhoneNumber("###-####"))
                .RuleFor(x => x.IsVerified, f => f.Random.Bool(0.7f))
                .RuleFor(x => x.Label, f => f.PickRandom(new[] { "Mobile", "Home", "Work", null }));
            db.CustomerPhones.AddRange(phoneFaker.Generate(2));
            await db.SaveChangesAsync(ct);
        }

        if (!await db.Invoices.AnyAsync(ct))
        {
            var internetSub = await db.InternetServiceSubscriptions
                .Include(s => s.InternetService)
                .FirstAsync(s => s.CustomerId == identityCustomerId && s.Status == SubscriptionStatus.Active, ct);
            var mobileSub = await db.MobileServiceSubscriptions
                .Include(s => s.MobileService)
                .FirstAsync(s => s.CustomerId == identityCustomerId && s.Status == SubscriptionStatus.Active, ct);
            var firstMonthUtc = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(-13);
            var firstMonth = new DateTimeOffset(firstMonthUtc, TimeSpan.Zero);
            var invCounter = 1000;
            for (int m = 0; m < 14; m++)
            {
                var issueDate = firstMonth.AddMonths(m);
                var dueDate = issueDate.AddDays(15);
                var invoice = new Invoice
                {
                    CustomerId = identityCustomerId,
                    InvoiceNumber = $"INV-{++invCounter}",
                    Status = InvoiceStatus.Pending,
                    IssueDate = issueDate,
                    DueDate = dueDate,
                    TotalAmount = 0m
                };
                db.Invoices.Add(invoice);
                var lines = new List<InvoiceDetail>
                {
                    new InvoiceDetail
                    {
                        InvoiceId = invoice.Id,
                        Invoice = invoice,
                        Description = $"Internet - {internetSub.InternetService.Name}",
                        Quantity = 1,
                        UnitPrice = internetSub.InternetService.MonthlyPrice,
                        LineTotal = internetSub.InternetService.MonthlyPrice
                    },
                    new InvoiceDetail
                    {
                        InvoiceId = invoice.Id,
                        Invoice = invoice,
                        Description = $"Mobile - {mobileSub.MobileService.Name}",
                        Quantity = 1,
                        UnitPrice = mobileSub.MobileService.MonthlyPrice,
                        LineTotal = mobileSub.MobileService.MonthlyPrice
                    }
                };
                db.InvoiceDetails.AddRange(lines);
                invoice.TotalAmount = lines.Sum(l => l.LineTotal);
                var markPaid = m < 13 && new Faker().Random.Bool(0.75f);
                if (markPaid)
                {
                    invoice.Status = InvoiceStatus.Paid;
                    db.Payments.Add(new Payment
                    {
                        InvoiceId = invoice.Id,
                        Invoice = invoice,
                        Amount = invoice.TotalAmount,
                        CreatedAt = issueDate.AddDays(new Faker().Random.Int(1, 12))
                    });
                }
            }
            await db.SaveChangesAsync(ct);
        }

        if (!await db.TechnicalRequests.AnyAsync(ct))
        {
            var ticketFaker = new Faker<TechnicalRequest>()
                .RuleFor(x => x.CustomerId, _ => identityCustomerId)
                .RuleFor(x => x.TicketNumber, f => $"TCK-{f.Random.Int(10000, 99999)}")
                .RuleFor(x => x.Subject, f => f.PickRandom(new[] {
                    "Router not syncing", "Intermittent connection drop", "Slow speed in evenings",
                    "Billing question", "ONUs light blinking"
                }))
                .RuleFor(x => x.Description, f => f.Lorem.Sentences(f.Random.Int(1, 3)))
                .RuleFor(x => x.Priority, f => f.PickRandom<TicketPriority>())
                .RuleFor(x => x.Status, f => f.Random.WeightedRandom(new[]
                {
                    TicketStatus.Open, TicketStatus.InProgress, TicketStatus.Closed
                }, [0.3f, 0.3f, 0.4f]))
                .RuleFor(x => x.CreatedAt, f => DateTimeOffset.UtcNow.AddDays(-f.Random.Int(5, 120)))
                .FinishWith((f, tr) =>
                {
                    if (tr.Status == TicketStatus.Closed)
                    {
                        tr.ResolvedAt = tr.CreatedAt.AddDays(f.Random.Int(1, 10));
                    }
                });
            db.TechnicalRequests.AddRange(ticketFaker.Generate(6));
            await db.SaveChangesAsync(ct);
        }
    }
}
