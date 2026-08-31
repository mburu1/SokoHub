namespace SokoHub.Contracts.Vendors;

public record VendorResponse(
    Guid Id,
    Guid UserId,
    string BusinessName,
    string TaxId,
    decimal CommissionRate,
    string Status);
