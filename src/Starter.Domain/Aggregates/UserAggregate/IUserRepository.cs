namespace Starter.Domain.Aggregates.UserAggregate;

public interface IUserRepository
{
    Task<User> CreateUser(User user);
    Task<User> ReadUser(Guid id);
    Task<User> ReadUser(string emailAddress, string hashedPassword);
    Task<User> UpdateUser(Guid id, User user);
}