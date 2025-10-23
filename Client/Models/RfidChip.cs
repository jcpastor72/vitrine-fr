namespace VitrineFr.Models;

public class RfidChip
{
    public Guid Id { get; set; }
    public string ChipId { get; set; } = string.Empty;
    public string Uid { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Label { get; set; }
    public string? Checksum { get; set; }
    public DateTime ActivationDate { get; set; }
    public DateTime? DeactivationDate { get; set; }

    public bool IsActive => DeactivationDate == null;
}
