namespace Starter.Domain;

public interface IDomainEvent
{
    public DateTime OccurredOn { get; }
    public Guid Id { get; }
}

public abstract class DomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.Now;
    public Guid Id { get; } = Guid.NewGuid();
}
