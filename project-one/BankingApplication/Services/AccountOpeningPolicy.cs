using BankingApp.Models;

namespace BankingApp.Services
{
    public static class AccountOpeningPolicy
    {
        public const decimal CheckingMinimumOpeningBalance = 0m;
        public const decimal CheckingOverdraftLimit = 0m;
        public const decimal SavingsMinimumOpeningBalance = 100m;
        public const decimal SavingsAnnualInterestRate = 0.02m;

        public static void ValidateOpeningBalance(
            AccountType accountType,
            decimal openingBalance)
        {
            decimal minimum = accountType switch
            {
                AccountType.Checking =>
                    CheckingMinimumOpeningBalance,
                AccountType.Savings =>
                    SavingsMinimumOpeningBalance,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(accountType))
            };

            if (openingBalance < minimum)
            {
                throw new ArgumentException(
                    $"{accountType} account requires an opening balance of at least {minimum:C}.");
            }
        }
    }
}
