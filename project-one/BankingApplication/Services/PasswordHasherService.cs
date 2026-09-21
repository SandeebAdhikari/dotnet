using BankingApp.Interfaces;
using BankingApp.Models;
using Microsoft.AspNetCore.Identity;

namespace BankingApp.Services
{
    public class PasswordHasherService : IPasswordHasher
    {
        private readonly PasswordHasher<User> _hasher;

        public PasswordHasherService()
        {
            _hasher = new PasswordHasher<User>();
        }

        public string Hash(
            User user,
            string password)
        {
            ValidatePassword(password);

            return _hasher.HashPassword(
                user,
                password);
        }

        public void ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) ||
                password.Length < 8)
            {
                throw new ArgumentException(
                    "Password must be at least 8 characters.");
            }
        }

        public bool Verify(
            User user,
            string passwordHash,
            string providedPassword)
        {
            if (string.IsNullOrWhiteSpace(providedPassword))
            {
                return false;
            }

            var result =
                _hasher.VerifyHashedPassword(
                    user,
                    passwordHash,
                    providedPassword);

            return result !=
                PasswordVerificationResult.Failed;
        }
    }
}
