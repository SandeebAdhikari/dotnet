namespace BankingApp.Services.Results
{
    public class AdminBankingSummary
    {
        public int TotalAccounts { get; init; }

        public int ActiveAccounts { get; init; }

        public int FrozenAccounts { get; init; }

        public int ClosedAccounts { get; init; }

        public decimal TotalBalance { get; init; }
    }
}
