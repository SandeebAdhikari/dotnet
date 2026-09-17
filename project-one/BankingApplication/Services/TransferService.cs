using BankingApp.Models;

namespace BankingApp.Services
{
    public class TransferService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;

        public TransferService(
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository)
        {
            this._accountRepository = accountRepository;
            this._transactionRepository = transactionRepository;
        }

        public void Transfer(
            int customerId,
            string fromAccountNumber,
            string toAccountNumber,
            decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException(
                    "Transfer amount must be greater than zero.");
            }

            Account fromAccount =
                _accountRepository.GetByAccountNumber(
                    fromAccountNumber);

            Account toAccount =
                _accountRepository.GetByAccountNumber(
                    toAccountNumber);

            if (fromAccount.AccountId ==
                toAccount.AccountId)
            {
                throw new InvalidOperationException(
                    "Cannot transfer money to the same account.");
            }

            if (fromAccount.CustomerId != customerId ||
                toAccount.CustomerId != customerId)
            {
                throw new UnauthorizedAccessException(
                    "Both accounts must belong to the customer.");
            }

            fromAccount.Withdraw(amount);

            toAccount.Deposit(amount);

            _accountRepository.Update(fromAccount);

            _accountRepository.Update(toAccount);

            Transaction transferOut =
                new Transaction(
                    fromAccount.AccountId,
                    amount,
                    TransactionType.TransferOut,
                    fromAccount.Balance,
                    toAccount.AccountId);

            Transaction transferIn =
                new Transaction(
                    toAccount.AccountId,
                    amount,
                    TransactionType.TransferIn,
                    toAccount.Balance,
                    fromAccount.AccountId);

            _transactionRepository.Create(transferOut);
            _transactionRepository.Create(transferIn);
        }
    }
}