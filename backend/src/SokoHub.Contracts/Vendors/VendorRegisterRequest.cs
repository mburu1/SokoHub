namespace SokoHub.Contracts.Vendors;

public record VendorRegisterRequest(
    Guid UserId,
    string BusinessName,
    string TaxId,
    decimal CommissionRate);
