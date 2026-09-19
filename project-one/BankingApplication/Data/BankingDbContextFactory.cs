using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BankingApp.Data
{
    public class BankingDbContextFactory
        : IDesignTimeDbContextFactory<BankingDbContext>
    {
        public BankingDbContext CreateDbContext(string[] args)
        {
            var connectionString =
                Environment.GetEnvironmentVariable(
                    "BANKING_DB_CONNECTION");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "BANKING_DB_CONNECTION environment variable is not set.");
            }

            var optionsBuilder =
                new DbContextOptionsBuilder<BankingDbContext>();

            optionsBuilder.UseSqlServer(connectionString);

            return new BankingDbContext(
                optionsBuilder.Options);
        }
    }
}