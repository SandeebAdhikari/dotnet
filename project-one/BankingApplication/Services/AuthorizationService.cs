using BankingApp.Interfaces;

namespace BankingApp.Services
{
    public class AuthorizationService
    {
        private readonly IUserRoleRepository _userRoleRepository;

        public AuthorizationService(
            IUserRoleRepository userRoleRepository)
        {
            _userRoleRepository = userRoleRepository;
        }

        public async Task<bool> IsCustomerAsync(int userId)
        {
            return await _userRoleRepository
                .UserHasRoleAsync(
                    userId,
                    "Customer");
        }

        public async Task<bool> IsAdminAsync(int userId)
        {
            return await _userRoleRepository
                .UserHasRoleAsync(
                    userId,
                    "Admin");
        }

        public async Task EnsureAdminAsync(int userId)
        {
            bool isAdmin =
                await IsAdminAsync(userId);

            if (!isAdmin)
            {
                throw new UnauthorizedAccessException(
                    "User is not authorized as an administrator.");
            }
        }
    }
}