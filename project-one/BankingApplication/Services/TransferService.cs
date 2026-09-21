using BankingApp.Interfaces;
using BankingApp.Models;

namespace BankingApp.Services
{
    public class TransferService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TransferService(
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository,
            IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task TransferAsync(
            int customerId,
            string fromAccountNumber,
            string toAccountNumber,
            decimal amount)
        {
            if (string.Equals(
                fromAccountNumber,
                toAccountNumber,
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Source and destination accounts must be different.");
            }

            await _unitOfWork.ExecuteInTransactionAsync(
                async () =>
                {
                    Account fromAccount =
                        await GetOwnedAccountAsync(
                            customerId,
                            fromAccountNumber);

                    Account toAccount =
                        await _accountRepository
                            .GetByAccountNumberAsync(
                                toAccountNumber)
                        ?? throw new InvalidOperationException(
                            "Destination account was not found.");

                    if (fromAccount.AccountId ==
                        toAccount.AccountId)
                    {
                        throw new InvalidOperationException(
                            "Source and destination accounts must be different.");
                    }

                    fromAccount.Withdraw(amount);
                    toAccount.Deposit(amount);

                    var transferOut = new Transaction(
                        fromAccount.AccountId,
                        amount,
                        TransactionType.TransferOut,
                        fromAccount.Balance,
                        toAccount.AccountId);

                    var transferIn = new Transaction(
                        toAccount.AccountId,
                        amount,
                        TransactionType.TransferIn,
                        toAccount.Balance,
                        fromAccount.AccountId);

                    await _transactionRepository
                        .AddRangeAsync(
                            new[]
                            {
                                transferOut,
                                transferIn
                            });

                    await _unitOfWork.SaveChangesAsync();
                });
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
