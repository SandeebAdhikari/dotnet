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
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public RegistrationService(
            IUserRepository userRepository,
            ICustomerRepository customerRepository,
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task RegisterCustomerAsync(
            string username,
            string password,
            string firstName,
            string lastName,
            DateTime dateOfBirth,
            Address address,
            string phoneNumber,
            string email,
            string ssn)
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

                    var userRole = new UserRole(
                        user.UserId,
                        customerRole.RoleId);

                    await _customerRepository
                        .AddAsync(customer);

                    await _userRoleRepository
                        .AddAsync(userRole);

                    await _unitOfWork.SaveChangesAsync();
                });
        }
    }
}