namespace MyISP.Maui.Models;

public enum InvoiceStatus
{
    Pending = 0,
    Paid = 1
}

public class InvoiceDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public InvoiceStatus Status { get; set; }
    public DateTimeOffset IssueDate { get; set; }
    public DateTimeOffset DueDate { get; set; }
    public decimal TotalAmount { get; set; }
}

public class MyInvoicesResponse
{
    public List<InvoiceDto> Pending { get; set; } = [];
    public List<InvoiceDto> Paid { get; set; } = [];
}
