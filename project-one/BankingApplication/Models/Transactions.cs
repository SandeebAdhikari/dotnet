namespace BankingApp.Models
{
    public class Transaction
    {
        public int TransactionId { get; private set; }

        public int AccountId { get; private set; }

        public int? RelatedAccountId { get; private set; }

        public decimal Amount { get; private set; }

        public TransactionType TransactionType { get; private set; }

        public DateTime TransactionDate { get; private set; }

        public decimal BalanceAfterTransaction { get; private set; }
        private Transaction(){}
        public Transaction(
            int accountId,
            decimal amount,
            TransactionType transactionType,
            decimal balanceAfterTransaction,
            int? relatedAccountId = null)
        {
            AccountId = accountId;
            Amount = amount;
            TransactionType = transactionType;
            BalanceAfterTransaction = balanceAfterTransaction;
            RelatedAccountId = relatedAccountId;

            TransactionDate = DateTime.UtcNow;
        }
    }
}