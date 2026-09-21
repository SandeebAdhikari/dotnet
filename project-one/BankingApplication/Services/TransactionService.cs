using BankingApp.Interfaces;
using BankingApp.Models;

namespace BankingApp.Services
{
    public class TransactionService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<IReadOnlyList<Transaction>>
            GetLastFiveTransactionsAsync(
                int customerId,
                string accountNumber)
        {
            Account account =
                await _accountRepository
                    .GetByAccountNumberAsync(accountNumber)
                ?? throw new InvalidOperationException(
                    "Account was not found.");

            if (account.CustomerId != customerId)
            {
                throw new UnauthorizedAccessException(
                    "Account does not belong to this customer.");
            }

            return await _transactionRepository
                .GetRecentByAccountIdAsync(
                    account.AccountId,
                    5);
        }
    }
}