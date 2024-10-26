namespace Starter.Domain.Aggregates.UserAggregate;

public sealed class CreatedUserEvent(User user) : DomainEvent
{
    public Guid UserId { get; private set; } = user.Id;
    public string Name { get; private set; } = $"{user.FirstName} {user.LastName}";
    public string EmailAddress { get; private set; } = user.EmailAddress;
}
