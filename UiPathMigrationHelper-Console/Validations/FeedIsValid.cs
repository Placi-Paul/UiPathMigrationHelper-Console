using System.ComponentModel.DataAnnotations;
using Cocona;

namespace UiPathMigrationHelper_Console.Validations
{
    internal class FeedIsValid : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string path && Uri.TryCreate(value.ToString(), UriKind.Absolute, out var _))
            {
                return ValidationResult.Success!;
            }

            return new ValidationResult($"The path '{value}' is not found.");
        }
    }
}
