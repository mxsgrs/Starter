namespace Starter.WebApi.UnitTests.Facts.Controllers;

public class UserControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly UserController _userController;

    public UserControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _userController = new UserController(_userServiceMock.Object);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnOkResult_WhenUserIsCreated()
    {
        // Arrange
        UserDto userDto = new()
        {
            Id = Guid.NewGuid(),
            EmailAddress = "test@example.com",
            HashedPassword = "hashedpassword",
            FirstName = "John",
            LastName = "Doe",
            Birthday = new DateOnly(1990, 1, 1),
            Gender = Gender.Male,
            Role = Role.User,
            Phone = "+1234567890",
            Address = new()
            {
                AddressLine = "123 Test St",
                City = "TestCity",
                ZipCode = "12345",
                Country = "TestCountry"
            }
        };

        _userServiceMock.Setup(s => s.CreateUser(It.IsAny<UserDto>())).ReturnsAsync(userDto);

        // Act
        IActionResult result = await _userController.CreateUser(userDto);

        // Assert
        OkObjectResult? okResult = Assert.IsType<OkObjectResult>(result);
        UserDto? returnedUserDto = Assert.IsType<UserDto>(okResult.Value);
        Assert.Equal(userDto, returnedUserDto);
        _userServiceMock.Verify(s => s.CreateUser(It.Is<UserDto>(u => u == userDto)), Times.Once);
    }

    [Fact]
    public async Task ReadUser_ShouldReturnNotFoundResult_WhenUserDoesNotExist()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _userServiceMock.Setup(s => s.ReadUser(userId)).ThrowsAsync(new UserNotFoundException(userId));

        // Act & Assert
        UserNotFoundException? exception = await Assert.ThrowsAsync<UserNotFoundException>(() => _userController.ReadUser(userId));
        Assert.Equal($"User {userId} was not found.", exception.Message);
        _userServiceMock.Verify(s => s.ReadUser(It.Is<Guid>(id => id == userId)), Times.Once);
    }

    [Fact]
    public async Task ReadUser_ShouldReturnOkResult_WhenUserExists()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        UserDto userDto = new()
        {
            Id = userId,
            EmailAddress = "test@example.com",
            HashedPassword = "hashedpassword",
            FirstName = "John",
            LastName = "Doe",
            Birthday = new DateOnly(1990, 1, 1),
            Gender = Gender.Male,
            Role = Role.User,
            Phone = "+1234567890",
            Address = new()
            {
                AddressLine = "123 Test St",
                City = "TestCity",
                ZipCode = "12345",
                Country = "TestCountry"
            }
        };

        _userServiceMock.Setup(s => s.ReadUser(userId)).ReturnsAsync(userDto);

        // Act
        IActionResult result = await _userController.ReadUser(userId);

        // Assert
        OkObjectResult? okResult = Assert.IsType<OkObjectResult>(result);
        UserDto? returnedUserDto = Assert.IsType<UserDto>(okResult.Value);
        Assert.Equal(userDto, returnedUserDto);
        _userServiceMock.Verify(s => s.ReadUser(It.Is<Guid>(id => id == userId)), Times.Once);
    }
}
