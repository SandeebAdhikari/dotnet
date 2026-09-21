using BankingApp.Interfaces;
using BankingApp.Models;

namespace BankingApp.Services
{
    public class RegistrationService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly AccountNumberGenerator
            _accountNumberGenerator;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IDataAnnotationsValidator
            _dataAnnotationsValidator;
        private readonly IUnitOfWork _unitOfWork;

        public RegistrationService(
            IUserRepository userRepository,
            ICustomerRepository customerRepository,
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository,
            IAccountRepository accountRepository,
            AccountNumberGenerator accountNumberGenerator,
            IPasswordHasher passwordHasher,
            IDataAnnotationsValidator dataAnnotationsValidator,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _accountRepository = accountRepository;
            _accountNumberGenerator = accountNumberGenerator;
            _passwordHasher = passwordHasher;
            _dataAnnotationsValidator =
                dataAnnotationsValidator;
            _unitOfWork = unitOfWork;
        }

        public async Task<Account> RegisterCustomerAsync(
            string username,
            string password,
            string firstName,
            string lastName,
            DateTime dateOfBirth,
            Address address,
            string phoneNumber,
            string email,
            string ssn,
            AccountType accountType,
            decimal openingBalance)
        {
            bool usernameExists =
                await _userRepository
                    .UsernameExistsAsync(username);

            if (usernameExists)
            {
                throw new InvalidOperationException(
                    "Username is already taken.");
            }

            Role customerRole =
                await _roleRepository
                    .GetByNameAsync("Customer")
                ?? throw new InvalidOperationException(
                    "Customer role was not found.");

            var user = new User
            {
                Username = username
            };

            user.PasswordHash =
                _passwordHasher.Hash(
                    user,
                    password);

            _dataAnnotationsValidator.Validate(user);
            _dataAnnotationsValidator.Validate(address);

            AccountOpeningPolicy.ValidateOpeningBalance(
                accountType,
                openingBalance);

            Account? openedAccount = null;

            await _unitOfWork.ExecuteInTransactionAsync(
                async () =>
                {
                    await _userRepository.AddAsync(user);

                    // Needed so SQL Server generates UserId.
                    await _unitOfWork.SaveChangesAsync();

                    var customer = new Customer(
                        user.UserId,
                        firstName,
                        lastName,
                        dateOfBirth,
                        address,
                        phoneNumber,
                        email,
                        ssn);

                    _dataAnnotationsValidator.Validate(customer);

                    var userRole = new UserRole(
                        user.UserId,
                        customerRole.RoleId);

                    await _customerRepository
                        .AddAsync(customer);

                    await _userRoleRepository
                        .AddAsync(userRole);

                    await _unitOfWork.SaveChangesAsync();

                    string accountNumber =
                        await _accountNumberGenerator.GenerateAsync();

                    openedAccount = accountType switch
                    {
                        AccountType.Checking =>
                            new CheckingAccount(
                                customer.CustomerId,
                                accountNumber,
                                openingBalance,
                                AccountOpeningPolicy
                                    .CheckingOverdraftLimit),

                        AccountType.Savings =>
                            new SavingsAccount(
                                customer.CustomerId,
                                accountNumber,
                                openingBalance,
                                AccountOpeningPolicy
                                    .SavingsAnnualInterestRate,
                                AccountOpeningPolicy
                                    .SavingsMinimumOpeningBalance),

                        _ => throw new ArgumentOutOfRangeException(
                            nameof(accountType))
                    };

                    await _accountRepository
                        .AddAsync(openedAccount);

                    await _unitOfWork.SaveChangesAsync();
                });

            return openedAccount
                ?? throw new InvalidOperationException(
                    "The account could not be created.");
        }
    }
}
