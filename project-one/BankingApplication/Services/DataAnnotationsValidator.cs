using System.ComponentModel.DataAnnotations;
using BankingApp.Interfaces;

namespace BankingApp.Services
{
    public class DataAnnotationsValidator
        : IDataAnnotationsValidator
    {
        public void Validate(object instance)
        {
            ArgumentNullException.ThrowIfNull(instance);

            var results = new List<ValidationResult>();
            var context = new ValidationContext(instance);

            bool isValid = Validator.TryValidateObject(
                instance,
                context,
                results,
                validateAllProperties: true);

            ThrowIfInvalid(isValid, results);
        }

        public void ValidatePropertyValue<T>(
            string propertyName,
            object? value)
        {
            var property = typeof(T).GetProperty(propertyName)
                ?? throw new ArgumentException(
                    $"Property '{propertyName}' was not found on {typeof(T).Name}.",
                    nameof(propertyName));

            var attributes = property
                .GetCustomAttributes(
                    typeof(ValidationAttribute),
                    inherit: true)
                .Cast<ValidationAttribute>()
                .ToArray();

            var results = new List<ValidationResult>();
            var context = new ValidationContext(new object())
            {
                MemberName = propertyName
            };

            bool isValid = Validator.TryValidateValue(
                value,
                context,
                results,
                attributes);

            ThrowIfInvalid(isValid, results);
        }

        private static void ThrowIfInvalid(
            bool isValid,
            IEnumerable<ValidationResult> results)
        {
            if (isValid)
            {
                return;
            }

            string message = string.Join(
                " ",
                results
                    .Select(result => result.ErrorMessage)
                    .Where(message =>
                        !string.IsNullOrWhiteSpace(message)));

            throw new ValidationException(message);
        }
    }
}
