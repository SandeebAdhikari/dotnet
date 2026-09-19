using BankingApp.Models;

namespace BankingApp.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNameAsync(string roleName);
    }
}