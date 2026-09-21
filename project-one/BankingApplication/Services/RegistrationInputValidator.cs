using BankingApp.Interfaces;
using BankingApp.Models;

namespace BankingApp.Services
{
    public class RegistrationInputValidator
    {
        private readonly IDataAnnotationsValidator
            _dataAnnotationsValidator;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public RegistrationInputValidator(
            IDataAnnotationsValidator dataAnnotationsValidator,
            IUserRepository userRepository,
            IPasswordHasher passwordHasher)
        {
            _dataAnnotationsValidator =
                dataAnnotationsValidator;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public string ValidateFirstName(string value)
        {
            return ValidateName(
                value,
                nameof(Customer.FirstName),
                "First name");
        }

        public string ValidateLastName(string value)
        {
            return ValidateName(
                value,
                nameof(Customer.LastName),
                "Last name");
        }

        public string ValidateStreet(string value)
        {
            return ValidateAddressValue(
                value,
                nameof(Address.Street));
        }

        public string ValidateCity(string value)
        {
            return ValidateAddressValue(
                value,
                nameof(Address.City));
        }

        public string ValidateState(string value)
        {
            return ValidateAddressValue(
                value,
                nameof(Address.State));
        }

        public string ValidateCountry(string value)
        {
            return ValidateAddressValue(
                value,
                nameof(Address.Country));
        }

        public string ValidateZipCode(string value)
        {
            return ValidateAddressValue(
                value,
                nameof(Address.ZipCode));
        }

        public string ValidatePhoneNumber(string value)
        {
            string trimmed = value.Trim();

            _dataAnnotationsValidator
                .ValidatePropertyValue<Customer>(
                    nameof(Customer.PhoneNumber),
                    trimmed);

            string digits = new string(
                trimmed.Where(char.IsDigit).ToArray());

            if (digits.Length != 10)
            {
                throw new ArgumentException(
                    "Phone number must contain 10 digits.");
            }

            return trimmed;
        }

        public string ValidateEmail(string value)
        {
            string trimmed = value.Trim();

            _dataAnnotationsValidator
                .ValidatePropertyValue<Customer>(
                    nameof(Customer.Email),
                    trimmed);

            return trimmed;
        }

        public string ValidateSocialSecurityNumber(
            string value)
        {
            string trimmed = value.Trim();
            string digits = trimmed.Replace("-", "");

            if (digits.Length != 9 ||
                !digits.All(char.IsDigit))
            {
                throw new ArgumentException(
                    "SSN must contain 9 digits.");
            }

            return trimmed;
        }

        public async Task<string> ValidateUsernameAsync(
            string value)
        {
            string trimmed = value.Trim();

            _dataAnnotationsValidator
                .ValidatePropertyValue<User>(
                    nameof(User.Username),
                    trimmed);

            if (await _userRepository
                .UsernameExistsAsync(trimmed))
            {
                throw new InvalidOperationException(
                    "Username is already taken.");
            }

            return trimmed;
        }

        public string ValidatePassword(string value)
        {
            _passwordHasher.ValidatePassword(value);
            return value;
        }

        public decimal ValidateOpeningBalance(
            AccountType accountType,
            decimal openingBalance)
        {
            AccountOpeningPolicy.ValidateOpeningBalance(
                accountType,
                openingBalance);

            return openingBalance;
        }

        private string ValidateName(
            string value,
            string propertyName,
            string displayName)
        {
            string trimmed = value.Trim();

            _dataAnnotationsValidator
                .ValidatePropertyValue<Customer>(
                    propertyName,
                    trimmed);

            if (!trimmed.All(character =>
                    char.IsLetter(character) ||
                    character == '\'' ||
                    character == '-' ||
                    character == ' '))
            {
                throw new ArgumentException(
                    $"{displayName} contains invalid characters.");
            }

            return trimmed;
        }

        private string ValidateAddressValue(
            string value,
            string propertyName)
        {
            string trimmed = value.Trim();

            _dataAnnotationsValidator
                .ValidatePropertyValue<Address>(
                    propertyName,
                    trimmed);

            return trimmed;
        }
    }
}
