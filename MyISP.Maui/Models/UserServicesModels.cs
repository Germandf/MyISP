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

public class ServiceSubscriptionDto<TService>
{
    public Guid Id { get; set; }
    public TService Service { get; set; } = default!;
}

public class MyServicesResponse
{
    public List<ServiceSubscriptionDto<InternetServiceDto>> Internet { get; set; } = [];
    public List<ServiceSubscriptionDto<MobileServiceDto>> Mobile { get; set; } = [];
}
