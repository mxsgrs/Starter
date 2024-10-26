using System.ComponentModel.DataAnnotations;

namespace Starter.Domain;

public interface IAggregateRoot { }

public abstract class AggregateRoot : IAggregateRoot
{
    // Events
    public static IReadOnlyCollection<IDomainEvent> DomainEvents => [.. _events];

    private static readonly List<IDomainEvent> _events = [];

    public void ClearDomainEvents() => _events.Clear();

    protected TDomainEvent RaiseDomainEvent<TDomainEvent>(TDomainEvent domainEvent)
        where TDomainEvent : IDomainEvent
    {
        _events.Add(domainEvent);
        return domainEvent;
    }

    // Self validation
    protected void Validate(object instance)
    {
        ValidationContext validationContext = new(instance);
        List<ValidationResult> validationResults = [];

        if (!Validator.TryValidateObject(instance, validationContext, validationResults, true))
        {
            throw new ValidationException($"{instance.GetType().Name} is not valid: " + string.Join(", ", validationResults));
        }
    }
}
