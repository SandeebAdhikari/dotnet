namespace BankingApp.Models
{
    public class SavingsAccount : Account
    {
        public override AccountType AccountType
        {
            get
            {
                return AccountType.Savings;
            }
        }

        public decimal AnnualInterestRate { get; private set; }

        public decimal MinimumBalance { get; private set; }
        private SavingsAccount() {}
        public SavingsAccount(
            int customerId,
            string accountNumber,
            decimal openingBalance,
            decimal annualInterestRate,
            decimal minimumBalance)
            : base(
                customerId,
                accountNumber,
                openingBalance)
        {
            if (annualInterestRate < 0 ||
                annualInterestRate > 1)
            {
                throw new ArgumentException(
                    "Annual interest rate must be between 0 and 1.");
            }

            if (minimumBalance < 0)
            {
                throw new ArgumentException(
                    "Minimum balance cannot be negative.");
            }

            if (openingBalance < minimumBalance)
            {
                throw new ArgumentException(
                    "Opening balance does not meet the minimum balance requirement.");
            }

            this.AnnualInterestRate = annualInterestRate;
            this.MinimumBalance = minimumBalance;
        }

        public override void Withdraw(decimal amount)
        {
            EnsureAccountIsActive();

            if (amount <= 0)
            {
                throw new ArgumentException(
                    "Withdrawal amount must be greater than zero.");
            }

            decimal balanceAfterWithdrawal =
                this.Balance - amount;

            if (balanceAfterWithdrawal < this.MinimumBalance)
            {
                throw new InvalidOperationException(
                    "Withdrawal would reduce the account below its minimum balance.");
            }

            this.Balance -= amount;
        }

        public decimal CalculateMonthlyInterest()
        {
            decimal monthlyInterestRate =
                this.AnnualInterestRate / 12;

            return this.Balance * monthlyInterestRate;
        }

        public void ApplyMonthlyInterest()
        {
            EnsureAccountIsActive();

            decimal interest =
                CalculateMonthlyInterest();

            if (interest > 0)
            {
                this.Balance += interest;
            }
        }

        public void UpdateTerms(
            decimal annualInterestRate,
            decimal minimumBalance)
        {
            if (annualInterestRate < 0 ||
                annualInterestRate > 1)
            {
                throw new ArgumentException(
                    "Annual interest rate must be between 0 and 1.");
            }

            if (minimumBalance < 0)
            {
                throw new ArgumentException(
                    "Minimum balance cannot be negative.");
            }

            if (this.Balance < minimumBalance)
            {
                throw new InvalidOperationException(
                    "Current balance does not meet the new minimum balance requirement.");
            }

            this.AnnualInterestRate = annualInterestRate;
            this.MinimumBalance = minimumBalance;
        }
    }
}
