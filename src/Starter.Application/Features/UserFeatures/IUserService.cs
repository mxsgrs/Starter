namespace Starter.Application.Features.UserFeatures;

public interface IUserService
{
    Task<UserDto> CreateUser(UserDto userDto);
    Task<UserDto> ReadUser(Guid id);
    Task<UserDto> ReadUser(string emailAddress, string hashedPassword);
    Task<UserDto> UpdateUser(Guid id, UserDto userDto);
}