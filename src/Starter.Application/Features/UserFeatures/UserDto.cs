using Starter.Domain.Aggregates.UserAggregate;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Starter.Application.Features.UserFeatures;

public record UserDto
{
    public Guid Id { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email address.")]
    public string EmailAddress { get; init; } = "";

    public string HashedPassword { get; init; } = "";
    public string FirstName { get; init; } = "";
    public string LastName { get; init; } = "";
    public DateOnly Birthday { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Gender Gender { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Role Role { get; init; }

    [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "The phone number must be between 10 and 15 digits and may include a leading +.")]
    public string Phone { get; init; } = "";

    public UserAddressDto Address { get; init; }
}

public record UserAddressDto
{
    public string AddressLine { get; init; } = "";
    public string? AddressSupplement { get; init; }
    public string City { get; init; } = "";
    public string ZipCode { get; init; } = "";
    public string? StateProvince { get; init; }
    public string Country { get; init; } = "";
}
