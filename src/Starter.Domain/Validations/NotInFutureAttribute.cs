using System.ComponentModel.DataAnnotations;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace Starter.Domain.Validations;

public class NotInFutureAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        bool isInTheFuture = value is DateTime dateTime && dateTime > DateTime.Now ||
            value is DateOnly dateOnly && dateOnly > DateOnly.FromDateTime(DateTime.Now);

        if (isInTheFuture)
        {
                return new($"{validationContext.MemberName} date can't be in the future.");
        }

        return ValidationResult.Success;
    }
}
