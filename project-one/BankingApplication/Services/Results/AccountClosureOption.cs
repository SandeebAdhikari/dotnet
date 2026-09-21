using BankingApp.Models;

namespace BankingApp.Services.Results
{
    public sealed class AccountClosureOption
    {
        public int AccountId { get; init; }

        public string MaskedAccountNumber { get; init; } = "";

        public AccountType AccountType { get; init; }

        public AccountStatus Status { get; init; }

        public decimal Balance { get; init; }

        public bool IsEligible { get; init; }

        public string? IneligibilityReason { get; init; }
    }
}
