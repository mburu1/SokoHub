namespace SokoHub.Contracts.Common;

public record AddressDto(
    string Street,
    string City,
    string County,
    string PostalCode,
    string Country = "Kenya");
