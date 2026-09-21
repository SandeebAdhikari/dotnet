using System.Security.Cryptography;
using BankingApp.Interfaces;

namespace BankingApp.Services
{
    public class AccountNumberGenerator
    {
        private readonly IAccountRepository _accountRepository;

        public AccountNumberGenerator(
            IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<string> GenerateAsync()
        {
            const int maximumAttempts = 10;

            for (int attempt = 0;
                attempt < maximumAttempts;
                attempt++)
            {
                string accountNumber =
                    RandomNumberGenerator
                        .GetInt32(
                            1_000_000_000,
                            2_000_000_000)
                        .ToString("D10");

                bool exists = await _accountRepository
                    .AccountNumberExistsAsync(accountNumber);

                if (!exists)
                {
                    return accountNumber;
                }
            }

            throw new InvalidOperationException(
                "A unique account number could not be generated.");
        }
    }
}
