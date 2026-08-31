namespace SokoHub.Contracts.Vendors;

public record VendorUpdateRequest(
    string BusinessName,
    decimal CommissionRate);
