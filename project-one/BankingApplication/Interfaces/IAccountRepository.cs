using BankingApp.Models;

namespace BankingApp.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(int accountId);

        Task<Account?> GetByAccountNumberAsync(
            string accountNumber);

        Task<IReadOnlyList<Account>> GetByCustomerIdAsync(
            int customerId);

        Task<bool> AccountNumberExistsAsync(
            string accountNumber);

        Task AddAsync(Account account);
    }
}