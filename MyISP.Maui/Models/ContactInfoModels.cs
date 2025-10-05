namespace MyISP.Maui.Models;

public class EmailDto
{
    public Guid Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public string? Label { get; set; }
}

public class PhoneDto
{
    public Guid Id { get; set; }
    public string AreaCode { get; set; } = string.Empty;
    public string LocalNumber { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public string? Label { get; set; }
}

public class MyContactInfoResponse
{
    public List<EmailDto> Emails { get; set; } = [];
    public List<PhoneDto> Phones { get; set; } = [];
}
