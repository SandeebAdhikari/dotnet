using BankingApp.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BankingApp.Data
{
    public class BankingDbContextFactory
        : IDesignTimeDbContextFactory<BankingDbContext>
    {
        public BankingDbContext CreateDbContext(string[] args)
        {
            string connectionString =
                BankingConfiguration.LoadConnectionString();

            var optionsBuilder =
                new DbContextOptionsBuilder<BankingDbContext>();

            optionsBuilder.UseSqlServer(connectionString);

            return new BankingDbContext(
                optionsBuilder.Options);
        }
    }
}
