namespace Starter.Infrastructure.UnitTests.Facts.Repositories;

public class UserRepositoryTests
{
    private readonly Mock<ILogger<UserRepository>> _logger = new();

    [Fact]
    public async Task CreateUser_ShouldAddUserToDatabase()
    {
        // Arrange
        StarterDbContext dbContext = SharedFixture.CreateDatabaseContext();
        User user = new(
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

        UserRepository repository = new(_logger.Object, dbContext);

        // Act
        User result = await repository.CreateUser(user);

        // Assert
        User? storedUser = await dbContext.Users.FindAsync(user.Id);
        Assert.NotNull(storedUser);
        Assert.Equal("test@example.com", storedUser.EmailAddress);
    }

    [Fact]
    public async Task ReadUser_ById_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        StarterDbContext dbContext = SharedFixture.CreateDatabaseContext();
        User user = new(
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

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        UserRepository repository = new(_logger.Object, dbContext);

        // Act
        User result = await repository.ReadUser(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.EmailAddress, result.EmailAddress);
    }

    [Fact]
    public async Task ReadUser_ById_ShouldThrow_WhenUserDoesNotExist()
    {
        // Arrange
        StarterDbContext dbContext = SharedFixture.CreateDatabaseContext();
        UserRepository repository = new(_logger.Object, dbContext);

        Guid userId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<UserNotFoundException>(() => repository.ReadUser(userId));
    }

    [Fact]
    public async Task ReadUser_ByEmailAndPassword_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        StarterDbContext dbContext = SharedFixture.CreateDatabaseContext();

        User user = new(
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

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        UserRepository repository = new(_logger.Object, dbContext);

        // Act
        var result = await repository.ReadUser("test@example.com", "hashedPassword");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }

    [Fact]
    public async Task ReadUser_ByEmailAndPassword_ShouldThrow_WhenUserDoesNotExist()
    {
        // Arrange
        StarterDbContext dbContext = SharedFixture.CreateDatabaseContext();
        UserRepository repository = new(_logger.Object, dbContext);

        string email = "nonexistent@example.com";
        string password = "wrongPassword";

        // Act & Assert
        await Assert.ThrowsAsync<UserNotFoundException>(() => repository.ReadUser(email, password));
    }

    [Fact]
    public async Task UpdateUser_ShouldUpdateUserDetails()
    {
        // Arrange
        StarterDbContext dbContext = SharedFixture.CreateDatabaseContext();

        User user = new(
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

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        User updatedUser = new(
            user.Id,
            "test@example.com",
            "hashedPassword",
            "Jane",
            "Doe",
            new DateOnly(1991, 2, 2),
            Gender.Female,
            Role.Admin,
            "+0987654321",
            new Address("New Street", "New City", "New State", "54321", "New Country")
        );

        UserRepository repository = new(_logger.Object, dbContext);

        // Act
        User result = await repository.UpdateUser(user.Id, updatedUser);

        // Assert
        Assert.Equal("Jane", result.FirstName);
        Assert.Equal("New Street", result.Address.AddressLine);
    }
}