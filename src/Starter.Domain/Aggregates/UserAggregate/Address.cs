namespace Starter.Domain.Aggregates.UserAggregate;

public class Address : ValueObject<Address>
{
    public string AddressLine { get; private set; } = "";
    public string? AddressSupplement { get; private set; }
    public string City { get; private set; } = "";
    public string ZipCode { get; private set; } = "";
    public string? StateProvince { get; private set; }
    public string Country { get; private set; } = "";

    public Address(string addressLine, string city, string zipCode, string country,
        string? addressSupplement = null, string? stateProvince = null)
    {
        AddressLine = addressLine;
        AddressSupplement = addressSupplement;
        City = city;
        ZipCode = zipCode;
        StateProvince = stateProvince;
        Country = country;

        Validate(this);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return AddressLine;
        yield return AddressSupplement;
        yield return City;
        yield return ZipCode;
        yield return StateProvince;
        yield return Country;
    }
}
