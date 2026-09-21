using BankingApp.Data;
using BankingApp.Interfaces;

namespace BankingApp.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BankingDbContext _context;

        public UnitOfWork(BankingDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task ExecuteInTransactionAsync(
            Func<Task> action)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                await action();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();

                _context.ChangeTracker.Clear();

                throw;
            }
        }
    }
}