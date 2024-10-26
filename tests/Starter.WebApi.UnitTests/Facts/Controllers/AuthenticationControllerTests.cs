namespace Starter.WebApi.UnitTests.Facts.Controllers;

public class AuthenticationControllerTests
{
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly AuthenticationController _authenticationController;

    public AuthenticationControllerTests()
    {
        _jwtServiceMock = new Mock<IJwtService>();
        _authenticationController = new AuthenticationController(_jwtServiceMock.Object);
    }

    [Fact]
    public async Task Token_ShouldReturnOkResult_WhenTokenIsCreated()
    {
        // Arrange
        HashedLoginRequest loginRequest = new()
        {
            EmailAddress = "test@example.com",
            HashedPassword = "hashedpassword"
        };

        LoginResponse expectedResponse = new()
        {
            AccessToken = "sample.jwt.token"
        };

        _jwtServiceMock.Setup(s => s.CreateToken(It.IsAny<HashedLoginRequest>())).ReturnsAsync(expectedResponse);

        // Act
        var result = await _authenticationController.Token(loginRequest);

        // Assert
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        LoginResponse returnedResponse = Assert.IsType<LoginResponse>(okResult.Value);
        Assert.Equal(expectedResponse.AccessToken, returnedResponse.AccessToken);
        _jwtServiceMock.Verify(s => s.CreateToken(It.Is<HashedLoginRequest>(req => req == loginRequest)), Times.Once);
    }

    [Fact]
    public async Task Token_ShouldReturnUnauthorized_WhenLoginFails()
    {
        // Arrange
        HashedLoginRequest loginRequest = new()
        {
            EmailAddress = "test@example.com",
            HashedPassword = "wrongpassword"
        };

        _jwtServiceMock.Setup(s => s.CreateToken(It.IsAny<HashedLoginRequest>()))
                       .ThrowsAsync(new UnauthorizedException());

        // Act & Assert
        UnauthorizedException exception = await Assert.ThrowsAsync<UnauthorizedException>(() => _authenticationController.Token(loginRequest));
        _jwtServiceMock.Verify(s => s.CreateToken(It.Is<HashedLoginRequest>(req => req == loginRequest)), Times.Once);
    }
}
