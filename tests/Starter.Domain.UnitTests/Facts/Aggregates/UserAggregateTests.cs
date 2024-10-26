using Starter.Domain.Aggregates.UserAggregate;
using System.ComponentModel.DataAnnotations;

namespace Starter.Domain.UnitTests.Facts.Aggregates;

public class UserTests
{
    [Fact]
    public void Constructor_WithFutureBirthday_ShouldThrowValidationException()
    {
        // Arrange
        DateOnly futureDate = DateOnly.FromDateTime(DateTime.Today.AddYears(1));

        // Act & Assert
        ValidationException exception = Assert.Throws<ValidationException>(() =>
            new User(Guid.NewGuid(), "test@example.com", "hashedPassword123", "John", "Doe", futureDate,
                Gender.Male, Role.User, "+1234567890", new Address("123 Main St", "City", "State", "PostalCode", "Country")));

        Assert.Equal("User is not valid: Birthday date can't be in the future.", exception.Message);
    }

    [Fact]
    public void Constructor_WithInvalidEmail_ShouldThrowValidationException()
    {
        // Arrange
        string invalidEmail = "invalid-email";

        // Act & Assert
        ValidationException exception = Assert.Throws<ValidationException>(() =>
            new User(Guid.NewGuid(), invalidEmail, "hashedPassword123", "John", "Doe", new DateOnly(1990, 1, 1),
                Gender.Male, Role.User, "+1234567890", new Address("123 Main St", "City", "State", "PostalCode", "Country")));

        Assert.Equal("User is not valid: Invalid email address.", exception.Message);
    }

    [Fact]
    public void Constructor_WithInvalidPhone_ShouldThrowValidationException()
    {
        // Arrange
        string invalidPhone = "123";

        // Act & Assert
        ValidationException exception = Assert.Throws<ValidationException>(() =>
            new User(Guid.NewGuid(), "test@example.com", "hashedPassword123", "John", "Doe", new DateOnly(1990, 1, 1),
                Gender.Male, Role.User, invalidPhone, new Address("123 Main St", "City", "State", "PostalCode", "Country")));

        Assert.Equal("User is not valid: The phone number must be between 10 and 15 digits and may include a leading +.", exception.Message);
    }

    [Fact]
    public void Constructor_WithLongFirstName_ShouldThrowValidationException()
    {
        // Arrange
        string longFirstName = new('a', 129); // More than 128 characters

        // Act & Assert
        ValidationException exception = Assert.Throws<ValidationException>(() =>
            new User(Guid.NewGuid(), "test@example.com", "hashedPassword123", longFirstName, "Doe", new DateOnly(1990, 1, 1),
                Gender.Male, Role.User, "+1234567890", new Address("123 Main St", "City", "State", "PostalCode", "Country")));

        Assert.Equal("User is not valid: The field FirstName must be a string or array type with a maximum length of '128'.", exception.Message);
    }

    [Fact]
    public void Constructor_WithValidInputs_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        string emailAddress = "test@example.com";
        string hashedPassword = "hashedPassword123";
        string firstName = "John";
        string lastName = "Doe";
        DateOnly birthday = new(1990, 1, 1);
        Gender gender = Gender.Male;
        Role role = Role.User;
        string phone = "+1234567890";
        Address address = new("123 Main St", "City", "State", "PostalCode", "Country");

        // Act
        User user = new(id, emailAddress, hashedPassword, firstName, 
            lastName, birthday, gender, role, phone, address);

        // Assert
        Assert.Equal(id, user.Id);
        Assert.Equal(emailAddress, user.EmailAddress);
        Assert.Equal(hashedPassword, user.HashedPassword);
        Assert.Equal(firstName, user.FirstName);
        Assert.Equal(lastName, user.LastName);
        Assert.Equal(birthday, user.Birthday);
        Assert.Equal(gender, user.Gender);
        Assert.Equal(role, user.Role);
        Assert.Equal(phone, user.Phone);
        Assert.Equal(address, user.Address);
    }
}
