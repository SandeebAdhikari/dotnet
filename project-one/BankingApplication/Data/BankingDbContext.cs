using BankingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.Data
{
    public class BankingDbContext : DbContext
    {
        public BankingDbContext(
            DbContextOptions<BankingDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        public DbSet<Customer> Customers => Set<Customer>();

        public DbSet<Role> Roles => Set<Role>();

        public DbSet<UserRole> UserRoles => Set<UserRole>();

        public DbSet<Account> Accounts => Set<Account>();

        public DbSet<Transaction> Transactions => Set<Transaction>();

        public DbSet<ServiceRequest> ServiceRequests
            => Set<ServiceRequest>();

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(BankingDbContext).Assembly);
        }
    }
}
