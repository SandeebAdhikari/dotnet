using System.ComponentModel.DataAnnotations;

namespace BankingApp.Models
{
    public abstract class Account
    {
        public int AccountId { get; private set; }

        public int CustomerId { get; private set; }

        [Required]
        [StringLength(20)]
        public string AccountNumber { get; private set; } = "";

        public abstract AccountType AccountType { get; }

        public decimal Balance { get; protected set; }

        public AccountStatus Status { get; private set; }

        public DateTime OpenedDate { get; private set; }
        protected Account(){}
        protected Account(
            int customerId,
            string accountNumber,
            decimal openingBalance = 0)
        {
            if (customerId <= 0)
            {
                throw new ArgumentException(
                    "Customer ID must be valid.");
            }

            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                throw new ArgumentException(
                    "Account number cannot be empty.");
            }

            if (openingBalance < 0)
            {
                throw new ArgumentException(
                    "Opening balance cannot be negative.");
            }

            this.CustomerId = customerId;
            this.AccountNumber = accountNumber;
            this.Balance = openingBalance;

            this.Status = AccountStatus.Active;
            this.OpenedDate = DateTime.UtcNow;
        }

        public virtual void Deposit(decimal amount)
        {
            EnsureAccountIsActive();

            if (amount <= 0)
            {
                throw new ArgumentException(
                    "Deposit amount must be greater than zero.");
            }

            this.Balance += amount;
        }

        public abstract void Withdraw(decimal amount);

        public void Freeze()
        {
            if (this.Status == AccountStatus.Closed)
            {
                throw new InvalidOperationException(
                    "Closed account cannot be frozen.");
            }

            this.Status = AccountStatus.Frozen;
        }

        public void Unfreeze()
        {
            if (this.Status != AccountStatus.Frozen)
            {
                throw new InvalidOperationException(
                    "Only a frozen account can be unfrozen.");
            }

            this.Status = AccountStatus.Active;
        }

        public void Close()
        {
            if (this.Balance != 0)
            {
                throw new InvalidOperationException(
                    "Account balance must be zero before closing.");
            }

            this.Status = AccountStatus.Closed;
        }

        protected void EnsureAccountIsActive()
        {
            if (this.Status != AccountStatus.Active)
            {
                throw new InvalidOperationException(
                    "Account is not active.");
            }
        }
    }
}