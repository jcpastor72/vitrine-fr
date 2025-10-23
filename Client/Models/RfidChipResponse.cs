namespace VitrineFr.Models;

public class RfidChipResponse
{
    public Guid Id { get; set; }
    public string ChipId { get; set; } = string.Empty;
    public string Uid { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ActivationDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? PackagingCode { get; set; }
}
