namespace MyISP.Maui.Models;

public class InternetServiceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Mbps { get; set; }
    public decimal MonthlyPrice { get; set; }
}

public class MobileServiceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Gbs { get; set; }
    public decimal MonthlyPrice { get; set; }
}

public class InternetSubscriptionDto
{
    public Guid Id { get; set; }
    public InternetServiceDto Service { get; set; } = default!;
}

public class MobileSubscriptionDto
{
    public Guid Id { get; set; }
    public MobileServiceDto Service { get; set; } = default!;
}

public class MyServicesResponse
{
    public List<InternetSubscriptionDto> Internet { get; set; } = [];
    public List<MobileSubscriptionDto> Mobile { get; set; } = [];
}
