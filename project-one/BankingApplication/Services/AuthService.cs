using BankingApp.Interfaces;
using BankingApp.Models;

namespace BankingApp.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<User?> AuthenticateAsync(
            string username,
            string password)
        {
            User? user =
                await _userRepository
                    .GetByUsernameAsync(username);

            if (user == null)
            {
                return null;
            }

            if (!user.IsActive)
            {
                return null;
            }

            bool passwordIsValid =
                _passwordHasher.Verify(
                    user,
                    user.PasswordHash,
                    password);

            if (!passwordIsValid)
            {
                return null;
            }

            return user;
        }

        public async Task ChangePasswordAsync(
            string username,
            string currentPassword,
            string newPassword)
        {
            User user =
                await _userRepository
                    .GetByUsernameAsync(username)
                ?? throw new InvalidOperationException(
                    "User was not found.");

            bool currentPasswordIsValid =
                _passwordHasher.Verify(
                    user,
                    user.PasswordHash,
                    currentPassword);

            if (!currentPasswordIsValid)
            {
                throw new InvalidOperationException(
                    "Current password is incorrect.");
            }

            user.PasswordHash =
                _passwordHasher.Hash(
                    user,
                    newPassword);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}