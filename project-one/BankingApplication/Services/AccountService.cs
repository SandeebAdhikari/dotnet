using BankingApp.Interfaces;
using BankingApp.Models;

namespace BankingApp.Services
{
    public class AccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository,
            IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<Account>>
            GetAccountsAsync(int customerId)
        {
            return await _accountRepository
                .GetByCustomerIdAsync(customerId);
        }

        public async Task<Account> DepositAsync(
            int customerId,
            string accountNumber,
            decimal amount)
        {
            Account account =
                await GetOwnedAccountAsync(
                    customerId,
                    accountNumber);

            account.Deposit(amount);

            var transaction = new Transaction(
                account.AccountId,
                amount,
                TransactionType.Deposit,
                account.Balance);

            await _transactionRepository
                .AddAsync(transaction);

            await _unitOfWork.SaveChangesAsync();

            return account;
        }

        public async Task<Account> WithdrawAsync(
            int customerId,
            string accountNumber,
            decimal amount)
        {
            Account account =
                await GetOwnedAccountAsync(
                    customerId,
                    accountNumber);

            account.Withdraw(amount);

            var transaction = new Transaction(
                account.AccountId,
                amount,
                TransactionType.Withdrawal,
                account.Balance);

            await _transactionRepository
                .AddAsync(transaction);

            await _unitOfWork.SaveChangesAsync();

            return account;
        }

        private async Task<Account> GetOwnedAccountAsync(
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

            return account;
        }
    }
}
