namespace LaborControl.Web.Models;

public class Client
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SubscriptionPlan { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsArchived { get; set; } = false;
    public DateTime? ArchivedAt { get; set; }
    public string? ArchivalReason { get; set; }
    public string? City { get; set; }
    public string? BusinessSector { get; set; }
}
