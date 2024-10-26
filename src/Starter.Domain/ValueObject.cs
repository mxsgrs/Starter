using System.ComponentModel.DataAnnotations;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace Starter.Domain;

public abstract class ValueObject<T> where T : ValueObject<T>
{
    public abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
        => obj is not null &&
           obj is T valueObject &&
           obj.GetType() == GetType() &&
           GetEqualityComponents().SequenceEqual(valueObject.GetEqualityComponents());

    public static bool operator ==(ValueObject<T> left, ValueObject<T> right)
        => left.Equals(right);

    public static bool operator !=(ValueObject<T> left, ValueObject<T> right)
        => !left.Equals(right);

    public override int GetHashCode()
        => GetEqualityComponents()
                .Select(x => x?.GetHashCode() ?? 0)
                .Aggregate((x, y) => x ^ y);

    protected void Validate(object instance)
    {
        ValidationContext validationContext = new(instance);
        List<ValidationResult> validationResults = [];

        if (!Validator.TryValidateObject(instance, validationContext, validationResults, true))
        {
            throw new ValidationException("User data is not valid: " + string.Join(", ", validationResults));
        }
    }
}
