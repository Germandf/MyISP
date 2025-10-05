namespace MyISP.Maui.Models;

public enum TicketStatus
{
    Open = 0,
    InProgress = 1,
    Closed = 2
}

public enum TicketPriority
{
    Low = 0,
    Medium = 1,
    High = 2
}

public class TechnicalRequestDto
{
    public Guid Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public TicketStatus Status { get; set; }
    public TicketPriority Priority { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ResolvedAt { get; set; }
}
