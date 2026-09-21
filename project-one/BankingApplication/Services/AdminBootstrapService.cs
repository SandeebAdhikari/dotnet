using BankingApp.Interfaces;
using BankingApp.Models;

namespace BankingApp.Services
{
    public class AdminBootstrapService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IDataAnnotationsValidator
            _dataAnnotationsValidator;
        private readonly IUnitOfWork _unitOfWork;

        public AdminBootstrapService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository,
            IPasswordHasher passwordHasher,
            IDataAnnotationsValidator dataAnnotationsValidator,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _passwordHasher = passwordHasher;
            _dataAnnotationsValidator =
                dataAnnotationsValidator;
            _unitOfWork = unitOfWork;
        }

        public async Task BootstrapAsync(
            string? username,
            string? password)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "BANKING_ADMIN_USERNAME and BANKING_ADMIN_PASSWORD must be set.");
            }

            string normalizedUsername = username.Trim();

            User? existingUser = await _userRepository
                .GetByUsernameAsync(normalizedUsername);

            if (existingUser != null)
            {
                bool alreadyAdmin = await _userRoleRepository
                    .UserHasRoleAsync(
                        existingUser.UserId,
                        "Admin");

                if (alreadyAdmin)
                {
                    return;
                }

                throw new InvalidOperationException(
                    "The configured administrator username already belongs to a non-admin user.");
            }

            Role adminRole = await _roleRepository
                .GetByNameAsync("Admin")
                ?? throw new InvalidOperationException(
                    "Admin role was not found.");

            var user = new User
            {
                Username = normalizedUsername
            };

            user.PasswordHash = _passwordHasher.Hash(
                user,
                password);

            _dataAnnotationsValidator.Validate(user);

            await _unitOfWork.ExecuteInTransactionAsync(
                async () =>
                {
                    await _userRepository.AddAsync(user);
                    await _unitOfWork.SaveChangesAsync();

                    var userRole = new UserRole(
                        user.UserId,
                        adminRole.RoleId);

                    await _userRoleRepository
                        .AddAsync(userRole);

                    await _unitOfWork.SaveChangesAsync();
                });
        }
    }
}
