using Starter.Application.UnitTests.Facts.Fixtures;

namespace Starter.Application.UnitTests.Facts.Features.AuthenticationFeatures;

public class JwtServiceTests : IClassFixture<SharedFixture>
{
    private readonly Mock<ILogger<JwtService>> _loggerMock = new();
    private readonly Mock<IConfiguration> _configurationMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly JwtService _jwtService;

    public JwtServiceTests(SharedFixture fixture)
    {
        _jwtService = new JwtService(
            _loggerMock.Object,
            fixture.Configuration,
            _userRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateToken_ShouldReturnToken_WhenUserIsValid()
    {
        // Arrange
        HashedLoginRequest hashedLoginRequest = new() 
        {
            EmailAddress = "test@example.com",
            HashedPassword = "hashedPassword"
        };

        User expectedUser = new(
            Guid.NewGuid(),
            "test@example.com",
            "hashedPassword",
            "John",
            "Doe",
            new DateOnly(1990, 1, 1),
            Gender.Male,
            Role.User,
            "+1234567890",
            new Address("Street", "City", "State", "12345", "Country")
        );

        JsonWebTokenParameters jwtParams = new()
        {
            Key = "a_secure_key_for_testing",
            Issuer = "test_issuer",
            Audience = "test_audience"
        };

        _userRepositoryMock.Setup(repo => repo.ReadUser(hashedLoginRequest.EmailAddress, hashedLoginRequest.HashedPassword))
            .ReturnsAsync(expectedUser);

        // Act
        LoginResponse result = await _jwtService.CreateToken(hashedLoginRequest);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.AccessToken);
    }

    [Fact]
    public async Task CreateToken_ShouldThrowUnauthorizedException_WhenUserIsInvalid()
    {
        // Arrange
        HashedLoginRequest hashedLoginRequest = new()
        {
            EmailAddress = "user@example.com",
            HashedPassword = "wrongPassword"
        };

        _userRepositoryMock.Setup(repo => repo.ReadUser(hashedLoginRequest.EmailAddress, hashedLoginRequest.HashedPassword))
            .ThrowsAsync(new UnauthorizedException());

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(() => _jwtService.CreateToken(hashedLoginRequest));
    }
}
