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

    // Relations avec autres entités
    public List<ZoneInfo> Sites { get; set; } = new();
    public List<AssetInfo> Assets { get; set; } = new();
    public List<RfidChip> RfidChips { get; set; } = new();
    public List<ClientUser> Users { get; set; } = new();
    public List<ClientOrder> Orders { get; set; } = new();
}

public class ClientUser
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}

public class ClientOrder
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeliveryDate { get; set; }
}
