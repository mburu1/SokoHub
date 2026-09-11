using SokoHub.Contracts.Common;

namespace SokoHub.Contracts.Customers;

public record CustomerResponse(
    Guid Id,
    Guid UserId,
    string Email,
    string Phone,
    IReadOnlyList<AddressDto>? Addresses = null);
