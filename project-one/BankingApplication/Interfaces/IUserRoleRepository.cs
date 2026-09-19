using BankingApp.Models;

namespace BankingApp.Interfaces
{
    public interface IUserRoleRepository
    {
        Task AddAsync(UserRole userRole);

        Task<bool> UserHasRoleAsync(
            int userId,
            string roleName);
    }
}