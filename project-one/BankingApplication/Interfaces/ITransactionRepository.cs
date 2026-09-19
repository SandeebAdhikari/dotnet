using BankingApp.Models;

namespace BankingApp.Interfaces
{
    public interface ITransactionRepository
    {
        Task<IReadOnlyList<Transaction>>
            GetRecentByAccountIdAsync(
                int accountId,
                int count);

        Task AddAsync(Transaction transaction);

        Task AddRangeAsync(
            IEnumerable<Transaction> transactions);
    }
}