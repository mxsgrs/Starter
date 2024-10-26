using AutoMapper;
using Starter.Application.Features.UserFeatures;

namespace Starter.Application.UnitTests.Facts.Features.UserFeatures;

public class UserServiceTests
{
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockLogger = new Mock<ILogger<UserService>>();
        _mockMapper = new Mock<IMapper>();
        _mockRepository = new Mock<IUserRepository>();
        _userService = new UserService(_mockLogger.Object, _mockMapper.Object, _mockRepository.Object);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnCreatedUserDto()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        UserDto userDto = new()
        {
            Id = id,
            EmailAddress = "test@example.com",
            HashedPassword = "hashedpassword",
            FirstName = "FirstName",
            LastName = "LastName",
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

        User user = new(id, "test@example.com", "hashedPassword", "FirstName", 
            "LastName", new DateOnly(1990, 1, 1), Gender.Male, Role.User, "+1234567890", 
            new("123 Test St", "TestCity", "12345", "TestCountry"));

        User createdUser = user;

        _mockMapper.Setup(m => m.Map<User>(userDto)).Returns(user);
        _mockRepository.Setup(r => r.CreateUser(user)).ReturnsAsync(createdUser);
        _mockMapper.Setup(m => m.Map<UserDto>(createdUser)).Returns(userDto);

        // Act
        UserDto result = await _userService.CreateUser(userDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userDto.EmailAddress, result.EmailAddress);
        Assert.Equal(userDto.HashedPassword, result.HashedPassword);
        Assert.Equal(userDto.FirstName, result.FirstName);
        Assert.Equal(userDto.LastName, result.LastName);
        Assert.Equal(userDto.Birthday, result.Birthday);
        Assert.Equal(userDto.Gender, result.Gender);
        Assert.Equal(userDto.Role, result.Role);
        Assert.Equal(userDto.Phone, result.Phone);
        Assert.Equal(userDto.Address.AddressLine, result.Address.AddressLine);
        Assert.Equal(userDto.Address.City, result.Address.City);
        Assert.Equal(userDto.Address.ZipCode, result.Address.ZipCode);
        Assert.Equal(userDto.Address.Country, result.Address.Country);
        _mockMapper.Verify(m => m.Map<User>(userDto), Times.Once);
        _mockRepository.Verify(r => r.CreateUser(user), Times.Once);
        _mockMapper.Verify(m => m.Map<UserDto>(createdUser), Times.Once);
    }

    [Fact]
    public async Task ReadUser_ByEmailAndPassword_ShouldReturnUserDto()
    {
        // Arrange
        string email = "test@example.com";
        string hashedPassword = "hashedPassword";
        Guid id = Guid.NewGuid();
        UserDto userDto = new()
        {
            Id = id,
            EmailAddress = "test@example.com",
            HashedPassword = "hashedpassword",
            FirstName = "FirstName",
            LastName = "LastName",
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

        User user = new(id, "test@example.com", "hashedPassword", "FirstName",
            "LastName", new DateOnly(1990, 1, 1), Gender.Male, Role.User, "+1234567890",
            new("123 Test St", "TestCity", "12345", "TestCountry"));

        _mockRepository.Setup(r => r.ReadUser(email, hashedPassword)).ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        // Act
        UserDto result = await _userService.ReadUser(email, hashedPassword);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userDto.Id, result.Id);
        Assert.Equal(userDto.EmailAddress, result.EmailAddress);
        Assert.Equal(userDto.HashedPassword, result.HashedPassword);
        Assert.Equal(userDto.FirstName, result.FirstName);
        Assert.Equal(userDto.LastName, result.LastName);
        Assert.Equal(userDto.Birthday, result.Birthday);
        Assert.Equal(userDto.Gender, result.Gender);
        Assert.Equal(userDto.Role, result.Role);
        Assert.Equal(userDto.Phone, result.Phone);
        Assert.Equal(userDto.Address.AddressLine, result.Address.AddressLine);
        Assert.Equal(userDto.Address.City, result.Address.City);
        Assert.Equal(userDto.Address.ZipCode, result.Address.ZipCode);
        Assert.Equal(userDto.Address.Country, result.Address.Country);
        _mockRepository.Verify(r => r.ReadUser(email, hashedPassword), Times.Once);
        _mockMapper.Verify(m => m.Map<UserDto>(user), Times.Once);
    }

    [Fact]
    public async Task ReadUser_ById_ShouldReturnUserDto()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        UserDto userDto = new()
        {
            Id = id,
            EmailAddress = "test@example.com",
            HashedPassword = "hashedpassword",
            FirstName = "FirstName",
            LastName = "LastName",
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

        User user = new(id, "test@example.com", "hashedPassword", "FirstName",
            "LastName", new DateOnly(1990, 1, 1), Gender.Male, Role.User, "+1234567890",
            new("123 Test St", "TestCity", "12345", "TestCountry"));

        _mockRepository.Setup(r => r.ReadUser(id)).ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        // Act
        UserDto result = await _userService.ReadUser(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userDto.Id, result.Id);
        Assert.Equal(userDto.EmailAddress, result.EmailAddress);
        Assert.Equal(userDto.HashedPassword, result.HashedPassword);
        Assert.Equal(userDto.FirstName, result.FirstName);
        Assert.Equal(userDto.LastName, result.LastName);
        Assert.Equal(userDto.Birthday, result.Birthday);
        Assert.Equal(userDto.Gender, result.Gender);
        Assert.Equal(userDto.Role, result.Role);
        Assert.Equal(userDto.Phone, result.Phone);
        Assert.Equal(userDto.Address.AddressLine, result.Address.AddressLine);
        Assert.Equal(userDto.Address.City, result.Address.City);
        Assert.Equal(userDto.Address.ZipCode, result.Address.ZipCode);
        Assert.Equal(userDto.Address.Country, result.Address.Country);
        _mockRepository.Verify(r => r.ReadUser(id), Times.Once);
        _mockMapper.Verify(m => m.Map<UserDto>(user), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnUpdatedUserDto()
    {

        Guid id = Guid.NewGuid();
        UserDto userDto = new()
        {
            Id = id,
            EmailAddress = "test@example.com",
            HashedPassword = "hashedpassword",
            FirstName = "FirstName",
            LastName = "LastName",
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

        User user = new(id, "test@example.com", "hashedPassword", "FirstName",
            "LastName", new DateOnly(1990, 1, 1), Gender.Male, Role.User, "+1234567890",
            new("123 Test St", "TestCity", "12345", "TestCountry"));

        User updatedUser = new(id, "test@example.com", "hashedPassword", "UpdatedFirstName",
            "UpdatedLastName", new DateOnly(1990, 1, 2), Gender.Female, Role.Admin, "+1234567891",
            new("124 Test St", "UpdatedTestCity", "12346", "UpdatedTestCountry"));

        UserDto updatedUserDto = new()
        {
            Id = id,
            EmailAddress = "test@example.com",
            HashedPassword = "hashedpassword",
            FirstName = "UpdatedFirstName",
            LastName = "UpdatedLastName",
            Birthday = new DateOnly(1990, 1, 2),
            Gender = Gender.Female,
            Role = Role.Admin,
            Phone = "+1234567891",
            Address = new()
            {
                AddressLine = "124 Test St",
                City = "UpdatedTestCity",
                ZipCode = "12346",
                Country = "UpdatedTestCountry"
            }
        };

        _mockMapper.Setup(m => m.Map<User>(userDto)).Returns(user);
        _mockRepository.Setup(r => r.UpdateUser(id, user)).ReturnsAsync(updatedUser);
        _mockMapper.Setup(m => m.Map<UserDto>(updatedUser)).Returns(updatedUserDto);

        // Act
        UserDto result = await _userService.UpdateUser(id, userDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedUserDto.Id, result.Id);
        Assert.Equal(updatedUserDto.EmailAddress, result.EmailAddress);
        Assert.Equal(updatedUserDto.HashedPassword, result.HashedPassword);
        Assert.Equal(updatedUserDto.FirstName, result.FirstName);
        Assert.Equal(updatedUserDto.LastName, result.LastName);
        Assert.Equal(updatedUserDto.Birthday, result.Birthday);
        Assert.Equal(updatedUserDto.Gender, result.Gender);
        Assert.Equal(updatedUserDto.Role, result.Role);
        Assert.Equal(updatedUserDto.Phone, result.Phone);
        Assert.Equal(updatedUserDto.Address.AddressLine, result.Address.AddressLine);
        Assert.Equal(updatedUserDto.Address.City, result.Address.City);
        Assert.Equal(updatedUserDto.Address.ZipCode, result.Address.ZipCode);
        Assert.Equal(updatedUserDto.Address.Country, result.Address.Country);
        _mockMapper.Verify(m => m.Map<User>(userDto), Times.Once);
        _mockRepository.Verify(r => r.UpdateUser(id, user), Times.Once);
        _mockMapper.Verify(m => m.Map<UserDto>(updatedUser), Times.Once);
    }
}
