namespace BankingApp.Models
{
    public class CheckingAccount : Account
    {
        public override AccountType AccountType
        {
            get
            {
                return AccountType.Checking;
            }
        }

        public decimal OverdraftLimit { get; private set; }

        public decimal AvailableFunds
        {
            get
            {
                return this.Balance + this.OverdraftLimit;
            }
        }
        private CheckingAccount() {}
        public CheckingAccount(
            int customerId,
            string accountNumber,
            decimal openingBalance,
            decimal overdraftLimit)
            : base(
                customerId,
                accountNumber,
                openingBalance)
        {
            if (overdraftLimit < 0)
            {
                throw new ArgumentException(
                    "Overdraft limit cannot be negative.");
            }

            this.OverdraftLimit = overdraftLimit;
        }

        public override void Withdraw(decimal amount)
        {
            EnsureAccountIsActive();

            if (amount <= 0)
            {
                throw new ArgumentException(
                    "Withdrawal amount must be greater than zero.");
            }

            if (amount > this.AvailableFunds)
            {
                throw new InvalidOperationException(
                    "Withdrawal exceeds available balance and overdraft limit.");
            }

            this.Balance -= amount;
        }

        public void UpdateOverdraftLimit(decimal overdraftLimit)
        {
            if (overdraftLimit < 0)
            {
                throw new ArgumentException(
                    "Overdraft limit cannot be negative.");
            }

            this.OverdraftLimit = overdraftLimit;
        }
    }
}
