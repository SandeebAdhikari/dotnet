using BankingApp.Data;
using BankingApp.Interfaces;
using BankingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly BankingDbContext _context;

        public AccountRepository(BankingDbContext context)
        {
            _context = context;
        }

        public async Task<Account?> GetByIdAsync(int accountId)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountId == accountId);
        }

        public async Task<Account?> GetByAccountNumberAsync(
            string accountNumber)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(
                    a => a.AccountNumber == accountNumber);
        }

        public async Task<IReadOnlyList<Account>> GetByCustomerIdAsync(
            int customerId)
        {
            return await _context.Accounts
                .Where(a => a.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<bool> AccountNumberExistsAsync(
            string accountNumber)
        {
            return await _context.Accounts
                .AnyAsync(a => a.AccountNumber == accountNumber);
        }

        public async Task<int> CountAsync()
        {
            return await _context.Accounts.CountAsync();
        }

        public async Task<int> CountByStatusAsync(
            AccountStatus status)
        {
            return await _context.Accounts
                .Where(a => a.Status == status)
                .CountAsync();
        }

        public async Task<decimal> GetTotalBalanceAsync()
        {
            return await _context.Accounts
                .Select(a => (decimal?)a.Balance)
                .SumAsync() ?? 0m;
        }

        public async Task AddAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
        }
    }
}
