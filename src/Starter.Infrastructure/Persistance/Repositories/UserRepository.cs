using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Starter.Domain.Aggregates.UserAggregate;

namespace Starter.Infrastructure.Persistance.Repositories;

public class UserRepository(ILogger<UserRepository> logger, StarterDbContext dbContext) : IUserRepository
{
    private readonly ILogger<UserRepository> _logger = logger;
    private readonly StarterDbContext _dbContext = dbContext;

    public async Task<User> CreateUser(User user)
    {
        _logger.LogInformation("Creating user credentials {User}", user);

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<User> ReadUser(Guid id)
    {
        User user = await _dbContext.Users.FindAsync(id)
            ?? throw new UserNotFoundException(id);

        return user;
    }

    public async Task<User> ReadUser(string emailAddress, string hashedPassword)
    {
        User user = await _dbContext.Users
            .FirstOrDefaultAsync(item => item.EmailAddress == emailAddress
                && item.HashedPassword == hashedPassword)
                    ?? throw new UserNotFoundException(emailAddress);

        return user;
    }

    public async Task<User> UpdateUser(Guid id, User user)
    {
        User existing = await _dbContext.Users
            .FirstOrDefaultAsync(item => item.EmailAddress == user.EmailAddress
                && item.HashedPassword == user.HashedPassword)
                    ?? throw new UserNotFoundException(user.EmailAddress);

        _logger.LogInformation("Updating user credentials {Existing}", existing);

        _dbContext.Entry(existing).CurrentValues.SetValues(user);

        // Update the user address as owned types are not updated by default
        if (user.Address is not null && existing.Address is not null)
        {
            _dbContext.Entry(existing.Address).CurrentValues.SetValues(user.Address);
        }

        await _dbContext.SaveChangesAsync();

        return existing;
    }
}
