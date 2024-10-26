using System.Net.Http.Json;

namespace Starter.WebApi.IntegrationTests.Facts.Controllers;

public class UserControllerTests(StarterWebApplicationFactory factory)
    : IClassFixture<StarterWebApplicationFactory>
{
    private readonly StarterWebApplicationFactory _factory = factory;

    [Fact]
    public async Task CreateUser_ShouldReturnOk_WhenUserIsCreated()
    {
        // Arrange
        _factory.MigrateDbContext();
        HttpClient client = _factory.CreateClient();

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

        string json = JsonSerializer.Serialize(userDto);
        StringContent content = new(json, Encoding.UTF8, "application/json");
        HttpRequestMessage request = new(HttpMethod.Post, "/api/user")
        {
            Content = content
        };

        // Act
        HttpResponseMessage response = await client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        UserDto? responseUserDto = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(responseUserDto);
        Assert.Equal("test@example.com", responseUserDto.EmailAddress);
        Assert.Equal("hashedpassword", responseUserDto.HashedPassword);
        Assert.Equal("FirstName", responseUserDto.FirstName);
        Assert.Equal("LastName", responseUserDto.LastName);
        Assert.Equal(new DateOnly(1990, 1, 1), responseUserDto.Birthday);
        Assert.Equal(Gender.Male, responseUserDto.Gender);
        Assert.Equal(Role.User, responseUserDto.Role);
        Assert.Equal("+1234567890", responseUserDto.Phone);
        Assert.Equal("123 Test St", responseUserDto.Address.AddressLine);
        Assert.Equal("TestCity", responseUserDto.Address.City);
        Assert.Equal("12345", responseUserDto.Address.ZipCode);
        Assert.Equal("TestCountry", responseUserDto.Address.Country);
    }

    [Fact]
    public async Task ReadUser_ShouldReturnOk_WhenUserExists()
    {
        // Arrange
        StarterDbContext dbContext = _factory.MigrateDbContext();

        Guid id = Guid.NewGuid();
        User user = new(id, "john.doe@example.com", "TWF0cml4UmVsb2FkZWQh", "FirstName",
            "LastName", new DateOnly(1990, 1, 1), Gender.Male, Role.User, "+1234567890",
            new("123 Test St", "TestCity", "12345", "TestCountry"));

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        HttpClient client = _factory.CreateAuthorizedClient();
        HttpRequestMessage request = new(HttpMethod.Get, $"/api/user/{id}");

        // Act
        HttpResponseMessage response = await client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        UserDto? responseUserDto = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(responseUserDto);
        Assert.Equal(id, responseUserDto.Id);
        Assert.Equal("john.doe@example.com", responseUserDto.EmailAddress);
        Assert.Equal("TWF0cml4UmVsb2FkZWQh", responseUserDto.HashedPassword);
        Assert.Equal("FirstName", responseUserDto.FirstName);
        Assert.Equal("LastName", responseUserDto.LastName);
        Assert.Equal(new DateOnly(1990, 1, 1), responseUserDto.Birthday);
        Assert.Equal(Gender.Male, responseUserDto.Gender);
        Assert.Equal(Role.User, responseUserDto.Role);
        Assert.Equal("+1234567890", responseUserDto.Phone);
        Assert.Equal("123 Test St", responseUserDto.Address.AddressLine);
        Assert.Equal("TestCity", responseUserDto.Address.City);
        Assert.Equal("12345", responseUserDto.Address.ZipCode);
        Assert.Equal("TestCountry", responseUserDto.Address.Country);
    }
}
