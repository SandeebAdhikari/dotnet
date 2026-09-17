using System.ComponentModel.DataAnnotations;

namespace BankingApp.Models
{
    public class Customer
    {
        private string _firstName = "";
        private string _lastName = "";
        private string _phoneNumber = "";
        private string _email = "";
        private string _socialSecurityNumber = "";

        public int CustomerId { get; private set; }

        public DateTime DateOfBirth { get; private set; }

        [Required]
        public Address Address { get; private set; }

        [Required]
        [StringLength(
            50,
            ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName
        {
            get
            {
                return _firstName;
            }

            set
            {
                if (value != null &&
                    !value.All(c =>
                        char.IsLetter(c) ||
                        c == '\'' ||
                        c == '-' ||
                        c == ' '))
                {
                    throw new ArgumentException(
                        "First name contains invalid characters.");
                }

                _firstName = value?.Trim() ?? "";
            }
        }

        [Required]
        [StringLength(
            50,
            ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName
        {
            get
            {
                return _lastName;
            }

            set
            {
                if (value != null &&
                    !value.All(c =>
                        char.IsLetter(c) ||
                        c == '\'' ||
                        c == '-' ||
                        c == ' '))
                {
                    throw new ArgumentException(
                        "Last name contains invalid characters.");
                }

                _lastName = value?.Trim() ?? "";
            }
        }

        [Required]
        [Phone]
        public string PhoneNumber
        {
            get
            {
                return _phoneNumber;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _phoneNumber = "";
                    return;
                }

                string cleanedPhone =
                    new string(value.Where(char.IsDigit).ToArray());

                if (cleanedPhone.Length != 10)
                {
                    throw new ArgumentException(
                        "Phone number must contain 10 digits.");
                }

                _phoneNumber = cleanedPhone;
            }
        }

        [Required]
        [EmailAddress]
        [StringLength(254)]
        public string Email
        {
            get
            {
                return _email;
            }

            set
            {
                _email = value?.Trim() ?? "";
            }
        }

        public string MaskedSocialSecurityNumber
        {
            get
            {
                if (_socialSecurityNumber.Length != 9)
                {
                    return "***-**-****";
                }

                return "***-**-" +
                       _socialSecurityNumber[^4..];
            }
        }

        public Customer(
            string firstName,
            string lastName,
            DateTime dateOfBirth,
            Address address,
            string phoneNumber,
            string email,
            string ssn)
        {
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Address = address;
            PhoneNumber = phoneNumber;
            Email = email;

            SetSocialSecurityNumber(ssn);
        }

        public void SetSocialSecurityNumber(string ssn)
        {
            if (string.IsNullOrWhiteSpace(ssn))
            {
                throw new ArgumentException(
                    "SSN cannot be empty.");
            }

            string cleanedSsn =
                ssn.Replace("-", "");

            if (cleanedSsn.Length != 9 ||
                !cleanedSsn.All(char.IsDigit))
            {
                throw new ArgumentException(
                    "SSN must contain 9 digits.");
            }

            _socialSecurityNumber = cleanedSsn;
        }
    }
}