using Microsoft.EntityFrameworkCore;

namespace MyISP.Bff;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<InternetService> InternetServices => Set<InternetService>();
    public DbSet<MobileService> MobileServices => Set<MobileService>();
    public DbSet<InternetServiceSubscription> InternetServiceSubscriptions => Set<InternetServiceSubscription>();
    public DbSet<MobileServiceSubscription> MobileServiceSubscriptions => Set<MobileServiceSubscription>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceDetail> InvoiceDetails => Set<InvoiceDetail>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<CustomerEmail> CustomerEmails => Set<CustomerEmail>();
    public DbSet<CustomerPhone> CustomerPhones => Set<CustomerPhone>();
    public DbSet<TechnicalRequest> TechnicalRequests => Set<TechnicalRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var customerId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        // Seed a single customer
        var demoCustomer = new Customer
        {
            Id = customerId,
            DisplayName = "Demo Customer"
        };

        // Seed services
        var net1Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var net2Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var mob1Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        var mob2Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");

        var net1 = new InternetService { Id = net1Id, Name = "Fiber 300 Mbps", Mbps = 300, MonthlyPrice = 29.99m };
        var net2 = new InternetService { Id = net2Id, Name = "Fiber 600 Mbps", Mbps = 600, MonthlyPrice = 39.99m };
        var mob1 = new MobileService { Id = mob1Id, Name = "Mobile 10 GB", Gbs = 10, MonthlyPrice = 9.99m };
        var mob2 = new MobileService { Id = mob2Id, Name = "Mobile 30 GB", Gbs = 30, MonthlyPrice = 14.99m };

        // Active subscriptions for the demo customer
        var netSubId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
        var mobSubId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

        modelBuilder.Entity<Customer>().HasData(demoCustomer);
        modelBuilder.Entity<InternetService>().HasData(net1, net2);
        modelBuilder.Entity<MobileService>().HasData(mob1, mob2);
        modelBuilder.Entity<InternetServiceSubscription>().HasData(new InternetServiceSubscription
        {
            Id = netSubId,
            CustomerId = customerId,
            InternetServiceId = net1Id,
            Status = SubscriptionStatus.Active,
            StartDate = DateTimeOffset.Parse("2024-01-01T00:00:00Z")
        });
        modelBuilder.Entity<MobileServiceSubscription>().HasData(new MobileServiceSubscription
        {
            Id = mobSubId,
            CustomerId = customerId,
            MobileServiceId = mob1Id,
            Status = SubscriptionStatus.Active,
            StartDate = DateTimeOffset.Parse("2024-01-01T00:00:00Z")
        });
    }
}

public class Customer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string DisplayName { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public abstract class ServiceBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required decimal MonthlyPrice { get; set; }
    public bool IsActive { get; set; } = true;
}

public class InternetService : ServiceBase
{
    public required int Mbps { get; set; }
}

public class MobileService : ServiceBase
{
    public required int Gbs { get; set; }
}

public abstract class ServiceSubscriptionBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
    public DateTimeOffset StartDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? EndDate { get; set; }
}

public enum SubscriptionStatus { Active = 0, Suspended = 1, Cancelled = 2 }

public class InternetServiceSubscription : ServiceSubscriptionBase
{
    public required Guid InternetServiceId { get; set; }
    public InternetService InternetService { get; set; } = default!;
}

public class MobileServiceSubscription : ServiceSubscriptionBase
{
    public required Guid MobileServiceId { get; set; }
    public MobileService MobileService { get; set; } = default!;
}

public class Invoice
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;
    public required string InvoiceNumber { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;
    public DateTimeOffset IssueDate { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset DueDate { get; set; } = DateTimeOffset.UtcNow.AddDays(30);
    public required decimal TotalAmount { get; set; }
}

public enum InvoiceStatus { Pending = 0, Paid = 1 }

public class InvoiceDetail
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = default!;
    public required string Description { get; set; }
    public required int Quantity { get; set; }
    public required decimal UnitPrice { get; set; }
    public required decimal LineTotal { get; set; }
}

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required Guid InvoiceId { get; set; }
    public required Invoice Invoice { get; set; } = default!;
    public decimal Amount { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public abstract class ContactInfoBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;
    public bool IsVerified { get; set; }
    public string? Label { get; set; }
}

public class CustomerEmail : ContactInfoBase
{
    public required string Address { get; set; }
}

public class CustomerPhone : ContactInfoBase
{
    public required string AreaCode { get; set; }
    public required string LocalNumber { get; set; }
}

public class TechnicalRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;
    public string TicketNumber { get; set; } = default!;
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public TicketPriority Priority { get; set; } = TicketPriority.Normal;
    public string Subject { get; set; } = default!;
    public string? Description { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ResolvedAt { get; set; }
}

public enum TicketStatus { Open = 0, InProgress = 1, Closed = 2 }

public enum TicketPriority { Low = 0, Normal = 1, High = 2, Urgent = 3 }
